var edge = require('edge')
  , path = require('path');

var docparse = function() {

  var parse = function(projects, callback) {
    var fn = edge.func('./src/NetDoc.Parser/bin/Debug/NetDoc.Parser.dll');
    for (var i = 0; i < projects.length; i++){
        projects[i].path = path.resolve(projects[i].path);
    }

    var options = {
      projects: projects
    }

    fn(options, function (err, result) { 
      if (err) {
        throw err;
      }
      var data = JSON.parse(result);
      callback(data);
    });
  }

  return {
    parse: parse
  }
}();

module.exports = docparse;