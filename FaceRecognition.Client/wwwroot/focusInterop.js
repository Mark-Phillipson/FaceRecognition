(function () {
  window.focusInterop = {
    focusElementById: function (id) {
      try {
        var el = document.getElementById(id);
        if (el && typeof el.focus === 'function') {
          el.focus();
          return true;
        }
      } catch (e) {
        // ignore
      }
      return false;
    }
  };
})();
