(function () {
  function tryLoadScript(url) {
    return new Promise(function (resolve, reject) {
      if (window.faceapi) { resolve(); return; }
      var s = document.createElement('script');
      s.src = url;
      s.onload = function () { resolve(); };
      s.onerror = function () { reject(new Error('Failed to load ' + url)); };
      document.head.appendChild(s);
    });
  }

  async function loadFaceApi() {
    if (window.faceapi) return;
    // Prefer a local copy first (avoid Tracking Protection issues), fallback to CDN
    const localPath = '/lib/face-api.min.js';
    try {
      // quick check if local file exists
      const r = await fetch(localPath, { method: 'HEAD' });
      if (r.ok) {
        await tryLoadScript(localPath);
        return;
      }
    } catch (e) {
      // ignore and fallback to CDN
    }

    const cdnUrl = 'https://unpkg.com/face-api.js@0.22.2/dist/face-api.min.js';
    await tryLoadScript(cdnUrl);
  }

  async function loadModels(modelBaseUrl) {
    if (!window.faceapi) throw new Error('face-api.js not loaded');
    // Try the provided model base URL (prefer local). If manifests not found, try a known upstream location.
    const fallbacks = [modelBaseUrl, 'https://raw.githubusercontent.com/justadudewhohacks/face-api.js/master/weights', 'https://unpkg.com/face-api.js@0.22.2/weights'];
    let lastErr = null;
    for (const base of fallbacks) {
      try {
        await faceapi.nets.ssdMobilenetv1.loadFromUri(base);
        await faceapi.nets.faceLandmark68Net.loadFromUri(base);
        await faceapi.nets.faceRecognitionNet.loadFromUri(base);
        return; // success
      } catch (e) {
        lastErr = e;
        // try next fallback
      }
    }
    throw lastErr || new Error('Failed to load face-api models');
  }

  async function detectSingleFaceEmbedding(dataUrl) {
    if (!window.faceapi) throw new Error('face-api.js not loaded');
    return new Promise(function (resolve, reject) {
      var img = new Image();
      img.crossOrigin = 'anonymous';
      img.onload = async function () {
        try {
          const result = await faceapi.detectSingleFace(img).withFaceLandmarks().withFaceDescriptor();
          if (!result) { resolve(null); return; }
          const descriptor = Array.from(result.descriptor || []);
          const box = result.detection && result.detection.box ? result.detection.box : { x: 0, y: 0, width: 0, height: 0 };
          resolve({ Descriptor: descriptor, Box: { X: box.x, Y: box.y, Width: box.width, Height: box.height } });
        } catch (e) {
          reject(e);
        }
      };
      img.onerror = function () { reject(new Error('Failed to load image for detection')); };
      img.src = dataUrl;
    });
  }

  window.faceInterop = {
    loadFaceApi: loadFaceApi,
    loadModels: loadModels,
    detectSingleFaceEmbedding: detectSingleFaceEmbedding
  };
})();

// detect all faces and return descriptors + cropped thumbnails
(function () {
  async function detectAllFacesEmbedding(dataUrl, thumbnailMaxSize) {
    // thumbnailMaxSize: if undefined or null -> default 160.
    // if <= 0 -> treat as "no scaling" (use original crop size)
    if (thumbnailMaxSize === undefined || thumbnailMaxSize === null) thumbnailMaxSize = 160;
    if (!window.faceapi) throw new Error('face-api.js not loaded');
    return new Promise(function (resolve, reject) {
      var img = new Image();
      img.crossOrigin = 'anonymous';
      img.onload = async function () {
        try {
          const detections = await faceapi.detectAllFaces(img).withFaceLandmarks().withFaceDescriptors();
          const results = [];
          const canvas = document.createElement('canvas');
          const ctx = canvas.getContext('2d');
          for (let i = 0; i < detections.length; i++) {
            const d = detections[i];
            const descriptor = Array.from(d.descriptor || []);
            const box = d.detection && d.detection.box ? d.detection.box : { x: 0, y: 0, width: 0, height: 0 };
            const pad = Math.max(box.width, box.height) * 0.25;
            let sx = Math.max(0, box.x - pad);
            let sy = Math.max(0, box.y - pad);
            let sw = Math.min(img.naturalWidth - sx, box.width + pad * 2);
            let sh = Math.min(img.naturalHeight - sy, box.height + pad * 2);
            let scale;
            if (thumbnailMaxSize <= 0) {
              scale = 1; // no scaling (use original crop size)
            } else {
              scale = Math.min(1, thumbnailMaxSize / Math.max(sw, sh));
            }
            const tw = Math.max(1, Math.round(sw * scale));
            const th = Math.max(1, Math.round(sh * scale));
            canvas.width = tw;
            canvas.height = th;
            ctx.clearRect(0, 0, tw, th);
            ctx.drawImage(img, sx, sy, sw, sh, 0, 0, tw, th);
            const thumbnail = canvas.toDataURL('image/jpeg', 0.9);
            results.push({ Descriptor: descriptor, Box: { X: box.x, Y: box.y, Width: box.width, Height: box.height }, ThumbnailDataUrl: thumbnail, ThumbnailWidth: tw, ThumbnailHeight: th });
          }
          resolve(results);
        } catch (e) {
          reject(e);
        }
      };
      img.onerror = function () { reject(new Error('Failed to load image for detection')); };
      img.src = dataUrl;
    });
  }

  // attach to existing faceInterop namespace
  if (!window.faceInterop) window.faceInterop = {};
  window.faceInterop.detectAllFacesEmbedding = detectAllFacesEmbedding;
})();

