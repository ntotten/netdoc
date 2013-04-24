# NetDoc
NetDoc is a tool for creating simple API documents from C# code.


## Installation
This project uses Node.js for the command line interface. To install use npm.

    npm install https://github.com/ntotten/netdoc/tarball/master/ -g

## Use

    // Generates docs from a csproj file
    netdoc <inputPath> <outputPath>

    example:
    netdoc .\Source\MyProject.csproj .\docs\


    // Create a json document use to generate the docs
    netdoc json <inputPath> <outputPath> 

    example:
    netdoc .\Source\MyProject.csproj .\docs\data.json


    // Generates docs from a json file
    netdoc gen <inputPath> <outputPath>

    example:
    netdoc .\docs\data.json .\docs\ 