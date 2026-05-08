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
          resolve({ descriptor: descriptor, box: { x: box.x, y: box.y, width: box.width, height: box.height } });
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
