
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
      var projects = [];
      projects.push({path: inputPath});
      docparse.parse(projects, function(data) {
        docgen.generate(data, outputPath);
      });
   });
 program
   .command('fromConfig <inputPath> <outputPath>')
   .description('generate docs for the given csproj')
   .action(function(inputPath, outputPath) {
      console.log('generating docs from "%s" config file', inputPath);

      var json = fs.readFileSync(inputPath, 'utf8');
      var projects = JSON.parse(json);

      docparse.parse(projects, function(data) {
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
      var projects = [];
      projects.push({path: inputPath});
      docparse.parse(projects, function(data) {
        console.log("Saving json data");
        var s = JSON.stringify(data, null, 2);
        fs.writeFileSync(outputPath, s, 'utf8');
      });
   });

 program
   .command('jsonFromConfig <inputPath> <outputPath>')
   .description('generate a documentation json file')
   .action(function(inputPath, outputPath) {
      console.log('generating docs from "%s" config file', inputPath);

      var json = fs.readFileSync(inputPath, 'utf8');
      var projects = JSON.parse(json);

      docparse.parse(projects, function(data) {
        console.log("Saving json data");
        var s = JSON.stringify(data, null, 2);
        fs.writeFileSync(outputPath, s, 'utf8');
      });
   });



exports = module.exports = program;