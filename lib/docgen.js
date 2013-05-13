var jade = require('jade')
  , joinPath = require('path').join
  , fs = require('fs');

var docgen = function() {
  var generate = function(data, path) {
    if (!fs.existsSync(path)) {
      fs.mkdirSync(path);
    }
    generateIndex(data);
    generateIndexPage(data, path);

    for (var i = data.namespaces.length - 1; i >= 0; i--) {
      var namespace = data.namespaces[i];
      namespace.index = data.index;
      generateNamespace(namespace, path);
    };
  },

  generateIndex = function(data) {
    var generalIndex = [];
    for (var i = data.namespaces.length - 1; i >= 0; i--) {
      var namespace = data.namespaces[i];
      var index = {};
      index.namespace = namespace.fullName;
      index.types = [];

      for (var j = namespace.namedTypes.length - 1; j >= 0; j--) {
        var type = {}
        type.fullName = namespace.namedTypes[j].fullName;
        type.name = namespace.namedTypes[j].name;
        
        index.types[j] = type
      };

      generalIndex[i] = index;
    };
    data.index = generalIndex;
  },
  
  generateIndexPage = function(data, path) {
    console.log('Index page');
    var templatePath = joinPath(__dirname, '../templates/indexTemplate.jade');
    var template = fs.readFileSync(templatePath, 'utf8');
    var fn = jade.compile(template, {filename: templatePath, pretty: true});

    // { name: "", summary: "", properties: [], methods: [] }
    var output = fn(data);
    var file = path  + 'index.html'
    console.log('Saving file %s...', file);
    fs.writeFileSync(file, output, 'utf8');
  },

  generateNamespace = function(data, path) {
    console.log('Namespace: %s', data.fullName);
    //data.name
    //data.summary

    //data.namedTypes
    for (var i = data.namedTypes.length - 1; i >= 0; i--) {
      var namedType = data.namedTypes[i];
      namedType.index = data.index;
      generateType(namedType, path);
    };
    
  },

  generateType = function(data, path) {
    console.log('Class: %s', data.fullName);

    var templatePath = joinPath(__dirname, '../templates/typeTemplate.jade');
    var template = fs.readFileSync(templatePath, 'utf8');
    var fn = jade.compile(template, {filename: templatePath, pretty: true});

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