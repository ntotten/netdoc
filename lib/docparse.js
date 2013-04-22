var edge = require('edge')
  , path = require('path');

var docparse = function() {

  var parse = function(directory, callback) {
    var fn = edge.func('./src/NetDoc.Parser/bin/Debug/NetDoc.Parser.dll');
    var dir = path.resolve(directory);
    var options = {
      path: dir
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