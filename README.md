# NetDoc
NetDoc is a tool for creating simple API documents from C# code.

## Installation
This project uses Node.js for the command line interface. Note that you need version 0.10.* or greater of Node.js.

To install use npm.

    npm install https://github.com/ntotten/netdoc/tarball/master/ -g

## Use

    // Generates docs from a csproj file
    netdoc <inputPath> <outputPath>

    example:
    netdoc .\Source\MyProject.csproj .\docs\


    // Create a json document use to generate the docs
    netdoc json <inputPath> <outputPath> 

    example:
    netdoc json .\Source\MyProject.csproj .\docs\data.json


    // Generates docs from a json file
    netdoc gen <inputPath> <outputPath>

    example:
    netdoc gen .\docs\data.json .\docs\ 


    // Generates docs from a config file
    netdoc fromConfig <inputPath> <outputPath>

    example:
    netdoc fromConfig config.json .\docs\


    // Create a json document use to generate the docs from a config file
    netdoc jsonFromConfig <inputPath> <outputPath> 

    example:
    netdoc jsonFromConfig config.json .\docs\data.json