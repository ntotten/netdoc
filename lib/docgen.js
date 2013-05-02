var jade = require('jade')
  , fs = require('fs');

var docgen = function() {
  var generate = function(data, path) {
    if (!fs.existsSync(path)) {
      fs.mkdirSync(path);
    }
    generateIndex(data, path);

    for (var i = data.namespaces.length - 1; i >= 0; i--) {
      var namespace = data.namespaces[i];
      namespace.generalIndex = data.index;
      generateNamespace(namespace, path);
    };
  },

  generateIndex = function(data, path) {
    var generalIndex = [];
    for (var i = data.namespaces.length - 1; i >= 0; i--) {
      var namespace = data.namespaces[i];
      var index = {};
      index.namespace = namespace.fullName;
      index.types = [];

      for (var j = namespace.namedTypes.length - 1; j >= 0; j--) {
        index.types[j] = namespace.namedTypes[j].name;
      };

      generalIndex[i] = index;
    };
    data.index = generalIndex;
  },

  generateNamespace = function(data, path) {
    console.log('Namespace: %s', data.fullName);
    //data.name
    //data.summary

    //data.namedTypes
    for (var i = data.namedTypes.length - 1; i >= 0; i--) {
      var namedType = data.namedTypes[i];
      namedType.generalIndex = data.generalIndex;
      generateType(namedType, path);
    };
    
  },

  generateType = function(data, path) {
    console.log('Class: %s', data.fullName);

    var templatePath =  './templates/typeTemplate.jade';
    var template = fs.readFileSync(templatePath, 'utf8');
    var fn = jade.compile(template, {filename: templatePath});

    // { name: "", summary: "", properties: [], methods: [] }
    var output = fn(data);
    var file = path + data.fullName + '.html'
    console.log('Saving file %s...', file);
    fs.writeFileSync(file, output, 'utf8');
  };

  return {
    generate: generate
  }
  
}();

module.exports = docgen;