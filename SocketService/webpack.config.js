const path = require("path");
let src = __dirname+"/src"
module.exports ={
    entry:{  server :"./src/server/server.js", client: "./src/client/client.js"},
    output:{
        filename:"[name].js",
        path: __dirname
    },
   devtool: 'inline-source-map',
   mode:"development",
   target:"node",
   node: {
    fs: 'empty'
    //net: 'empty',
    //tls: 'empty'
  },
  module: {
    rules: [
        {
            test: /\.js$/,
            exclude: /node_modules/,
            loader: "babel-loader", // or just "babel"
            
            
        }
    ]
},
stats: {
    warnings: false
  }

}