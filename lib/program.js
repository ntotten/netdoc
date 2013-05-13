
var commander = require('commander')
  , fs = require('fs')
  , docparse = require('./docparse')
  , docgen = require('./docgen');


var program = new commander.Command();


 program
   .version('0.0.1');

 program
   .command('* <inputPath> <outputPath>')
   .description('generate docs for the given csproj')
   .action(function(inputPath, outputPath) {
      console.log('generating docs from "%s"', inputPath);

      var tempPath = '.tempConfig.json';
      generateTempFile(inputPath, tempPath);
      docparse.parse(tempPath, function(data) {
        docgen.generate(data, outputPath);
      });
   });

 program
   .command('fromConfig <inputPath> <outputPath>')
   .description('generate docs for the given config file')
   .action(function(inputPath, outputPath) {
      console.log('generating docs from "%s" config file', inputPath);

      docparse.parse(inputPath, function(data) {
        docgen.generate(data, outputPath);
      });
   });

 program
   .command('gen <inputPath> <outputPath>')
   .description('generate docs from a generated json file')
   .action(function(inputPath, outputPath) {
      console.log('generating docs from "%s"', inputPath);

      var json = fs.readFileSync(inputPath, 'utf8');
      var data = JSON.parse(json);
      docgen.generate(data, outputPath);
   });

 program
   .command('json <inputPath> <outputPath>')
   .description('generate a documentation json file')
   .action(function(inputPath, outputPath) {
      console.log('generating json from "%s"', inputPath);

      var tempPath = '.tempConfig.json';
      generateTempFile(inputPath, tempPath);
      docparse.parse(tempPath, function(data) {
        console.log("Saving json data");
        var s = JSON.stringify(data, null, 2);
        fs.writeFileSync(outputPath, s, 'utf8');
      });
   });

 program
   .command('jsonFromConfig <inputPath> <outputPath>')
   .description('generate a documentation json file from the given config file')
   .action(function(inputPath, outputPath) {
      console.log('generating docs from "%s" config file', inputPath);

      docparse.parse(inputPath, function(data) {
        console.log("Saving json data");
        var s = JSON.stringify(data, null, 2);
        fs.writeFileSync(outputPath, s, 'utf8');
      });
   });

   
   var generateTempFile = function() {
      var config = {
        "projects" : [
          {
            "path": inputPath,
            "id": ""
          }
        ],
        "filteredNamespaces": []
      };
      var s = JSON.stringify(config, null, 2);
      var tempPath = '.tempConfig.json';
      fs.writeFileSync(tempPath, s, 'utf8');
   }


exports = module.exports = program;