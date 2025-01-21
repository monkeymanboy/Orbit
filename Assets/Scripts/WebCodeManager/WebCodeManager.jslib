mergeInto(LibraryManager.library, {

    SetActive: function (xml, csharp) {
        window.orbitEditor.setValue(UTF8ToString(xml));
        window.csharpEditor.setValue(UTF8ToString(csharp));
    },

    SubscribeToCodeChanged: function (objectName, callbackName) {
        var gameObject = UTF8ToString(objectName);
        var callback = UTF8ToString(callbackName);
        window.orbitEditor.onDidChangeModelContent((a) => {
            console.log(gameObject);
            console.log(callback);
            SendMessage(gameObject, callback, window.orbitEditor.getValue());
        });
    },

});