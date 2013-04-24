
var commander = require('commander')
  , fs = require('fs')
  , docparse = require('./docparse')
  , docgen = require('./docgen');


var program = new commander.Command();


 program
   .version('0.0.1')
   .option('-J, --json <path>', 'Save the document json to a file.');

 program
   .command('* <inputPath> <outputPath>')
   .description('generate docs for the given xml file')
   .action(function(inputPath, outputPath) {
      console.log('generating docs from "%s"', inputPath);
      docparse.parse(inputPath, function(data) {
        console.log(program.json)
        if (program.json) {
          console.log("Saving json data");
          var s = JSON.stringify(data);
          fs.writeFileSync(program.json, s, 'utf8');
        }
        docgen.generate(data, outputPath);
      });
   });



exports = module.exports = program;