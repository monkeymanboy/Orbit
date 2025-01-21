const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');
const path = require('path');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = {
    entry: './src/index.js',
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'index.js'
    },
    module: {
        rules: [
            { test: /\.css$/, use: ['style-loader', 'css-loader'] },
            { test: /\.ttf$/, use: ['file-loader'] },
            { test: /\.xsd$/, use: 'raw-loader' }
        ]
    },
    plugins: [
	new MonacoWebpackPlugin(),
    new CopyPlugin({
      patterns: [
        { from: "./src/index.html", to: "./index.html" }
      ],
    })
	]
};