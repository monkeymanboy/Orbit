import * as monaco from 'monaco-editor'
import orbitSchema from '../../OrbitSchema.xsd'
import XsdManager from 'monaco-xsd-code-completion/esm/XsdManager'
import XsdFeatures from 'monaco-xsd-code-completion/esm/XsdFeatures'
import 'monaco-xsd-code-completion/src/style.css'

window.orbitEditor = monaco.editor.create(document.getElementById('container-xml'), {
    value: '',
    language: 'xml',
    theme: 'vs-dark',
	automaticLayout: true,
	minimap: {enabled: false},
	suggestOnTriggerCharacters: true // show suggestions when we type one of the trigger characters
})
window.csharpEditor = monaco.editor.create(document.getElementById('container-csharp'), {
    value: '',
    language: 'csharp',
    theme: 'vs-dark',
	automaticLayout: true,
	minimap: {enabled: false},
})

const xsdManager = new XsdManager(window.orbitEditor) // Initialise the xsdManager
xsdManager.set({
    path: 'OrbitSchema.xsd',
    value: orbitSchema,
    nonStrictPath: true,
    alwaysInclude: true
})

const xsdFeatures = new XsdFeatures(xsdManager, monaco, window.orbitEditor) // Initialise the xsdFeatures.

xsdFeatures.addCompletion() // Add auto completion.
xsdFeatures.addValidation() // Add auto validation on debounce. Can be manually triggered with doValidation.
xsdFeatures.addReformatAction() // Add reformat code to actions menu. Can be run manually with doReformatCode.
