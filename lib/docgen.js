var jade = require('jade')
  , fs = require('fs');

var docgen = function() {
  var generate = function(data, path) {
    if (!fs.existsSync(path)) {
      fs.mkdirSync(path);
    }
    generateIndex(data, path);

    for (var i = data.namespaces.length - 1; i >= 0; i--) {
      generateNamespace(data.namespaces[i], path);
    };
  },

  generateIndex = function(data, path) {

  },

  generateNamespace = function(data, path) {
    console.log('Namespace: %s', data.fullName);
    //data.name
    //data.summary

    //data.namedTypes
    for (var i = data.namedTypes.length - 1; i >= 0; i--) {
      generateType(data.namedTypes[i], path);
    };
    
  },

  generateType = function(data, path) {
    console.log('Class: %s', data.fullName);

    var templatePath =  './templates/layout.jade';
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