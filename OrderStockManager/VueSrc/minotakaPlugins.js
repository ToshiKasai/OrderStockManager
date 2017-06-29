var minotakaFunctions = (function () {
  var minotakaFunctions = function () {
    if (!(this instanceof minotakaFunctions)) {
      return new minotakaFunctions();
    }
  }
  minotakaFunctions.prototype.typeOf = function (obj) {
    var toString = Object.prototype.toString
    return toString.call(obj).slice(8, -1).toLowerCase()
  }
  minotakaFunctions.prototype.checkType = function (obj, types) {
    return this.typeOf(obj) === types.toLowerCase()
  }
  minotakaFunctions.prototype.isNaN = function (obj) {
    return typeof obj === 'number' && obj !== obj
  }
  minotakaFunctions.prototype.makeArray = function (values, types) {
    if (types === null || types === undefined) {
      types = "object"
    }
    if (Array.isArray(values)) {
      return values
    } else if (this.typeOf(values) === types.toLowerCase()) {
      return [values]
    } else {
      return []
    }
  }
  minotakaFunctions.prototype.makeSingleType = function (value, types) {
    if (types === null || types === undefined) {
      types = "string"
    }
    if (this.checkType(value, types)) {
      return value
    } else {
      return null
    }
  }
  return minotakaFunctions;
})();

let minotakaPlugins = {
  install(Vue, options) {
    Vue.minotaka = minotakaFunctions();
    /*Vue.mixin({
      methods: {
        typeOf(obj) {
          var toString = Object.prototype.toString
          return toString.call(obj).slice(8, -1).toLowerCase()
        },
        isNaN(obj) {
          return typeof obj === 'number' && obj !== obj
        }
      }
    })*/
    Object.defineProperties(Vue.prototype, {
      minotaka: {
        get: function get() {
          return minotakaFunctions();
        }
      },
      $minotaka: {
        get: function get() {
          return minotakaFunctions();
        }
      }
    });
  }
};

export default minotakaPlugins;
