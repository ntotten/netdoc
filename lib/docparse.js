var exec = require('child_process').exec
  , path = require('path');

var docparse = function() {
  var parse = function(configFile, callback) {

    var netdocExe = path.join(__dirname, '../parser/NetDoc.exe');
    var configPath = path.resolve(configFile)
    exec( netdocExe + ' ' + configPath, { maxBuffer: 1024*1024 },
      function (error, stdout, stderr) {
        if (error !== null) {
          console.log('exec error: ' + error);
        } else {
          var data = JSON.parse(stdout);
          callback(data);
        }
      });
  }

  return {
    parse: parse
  }
}();

module.exports = docparse;