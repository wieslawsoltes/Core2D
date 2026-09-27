//! Licensed to the .NET Foundation under one or more agreements.
//! The .NET Foundation licenses this file to you under the MIT license.

var e=!1;const t=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,4,1,96,0,0,3,2,1,0,10,8,1,6,0,6,64,25,11,11])),o=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,15,1,13,0,65,1,253,15,65,2,253,15,253,128,2,11])),n=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,10,1,8,0,65,0,253,15,253,98,11])),r=Symbol.for("wasm promise_control");function i(e,t){let o=null;const n=new Promise((function(n,r){o={isDone:!1,promise:null,resolve:t=>{o.isDone||(o.isDone=!0,n(t),e&&e())},reject:e=>{o.isDone||(o.isDone=!0,r(e),t&&t())}}}));o.promise=n;const i=n;return i[r]=o,{promise:i,promise_control:o}}function s(e){return e[r]}function a(e){e&&function(e){return void 0!==e[r]}(e)||Be(!1,"Promise is not controllable")}const l="__mono_message__",c=["debug","log","trace","warn","info","error"],d="MONO_WASM: ";let u,f,m,g,p,h;function w(e){g=e}function b(e){if(Pe.diagnosticTracing){const t="function"==typeof e?e():e;console.debug(d+t)}}function y(e,...t){console.info(d+e,...t)}function v(e,...t){console.info(e,...t)}function E(e,...t){console.warn(d+e,...t)}function _(e,...t){if(t&&t.length>0&&t[0]&&"object"==typeof t[0]){if(t[0].silent)return;if(t[0].toString)return void console.error(d+e,t[0].toString())}console.error(d+e,...t)}function x(e,t,o){return function(...n){try{let r=n[0];if(void 0===r)r="undefined";else if(null===r)r="null";else if("function"==typeof r)r=r.toString();else if("string"!=typeof r)try{r=JSON.stringify(r)}catch(e){r=r.toString()}t(o?JSON.stringify({method:e,payload:r,arguments:n.slice(1)}):[e+r,...n.slice(1)])}catch(e){m.error(`proxyConsole failed: ${e}`)}}}function j(e,t,o){f=t,g=e,m={...t};const n=`${o}/console`.replace("https://","wss://").replace("http://","ws://");u=new WebSocket(n),u.addEventListener("error",A),u.addEventListener("close",S),function(){for(const e of c)f[e]=x(`console.${e}`,T,!0)}()}function R(e){let t=30;const o=()=>{u?0==u.bufferedAmount||0==t?(e&&v(e),function(){for(const e of c)f[e]=x(`console.${e}`,m.log,!1)}(),u.removeEventListener("error",A),u.removeEventListener("close",S),u.close(1e3,e),u=void 0):(t--,globalThis.setTimeout(o,100)):e&&m&&m.log(e)};o()}function T(e){u&&u.readyState===WebSocket.OPEN?u.send(e):m.log(e)}function A(e){m.error(`[${g}] proxy console websocket error: ${e}`,e)}function S(e){m.debug(`[${g}] proxy console websocket closed: ${e}`,e)}function D(){Pe.preferredIcuAsset=O(Pe.config);let e="invariant"==Pe.config.globalizationMode;if(!e)if(Pe.preferredIcuAsset)Pe.diagnosticTracing&&b("ICU data archive(s) available, disabling invariant mode");else{if("custom"===Pe.config.globalizationMode||"all"===Pe.config.globalizationMode||"sharded"===Pe.config.globalizationMode){const e="invariant globalization mode is inactive and no ICU data archives are available";throw _(`ERROR: ${e}`),new Error(e)}Pe.diagnosticTracing&&b("ICU data archive(s) not available, using invariant globalization mode"),e=!0,Pe.preferredIcuAsset=null}const t="DOTNET_SYSTEM_GLOBALIZATION_INVARIANT",o=Pe.config.environmentVariables;if(void 0===o[t]&&e&&(o[t]="1"),void 0===o.TZ)try{const e=Intl.DateTimeFormat().resolvedOptions().timeZone||null;e&&(o.TZ=e)}catch(e){y("failed to detect timezone, will fallback to UTC")}}function O(e){var t;if((null===(t=e.resources)||void 0===t?void 0:t.icu)&&"invariant"!=e.globalizationMode){const t=e.applicationCulture||(ke?globalThis.navigator&&globalThis.navigator.languages&&globalThis.navigator.languages[0]:Intl.DateTimeFormat().resolvedOptions().locale),o=e.resources.icu;let n=null;if("custom"===e.globalizationMode){if(o.length>=1)return o[0].name}else t&&"all"!==e.globalizationMode?"sharded"===e.globalizationMode&&(n=function(e){const t=e.split("-")[0];return"en"===t||["fr","fr-FR","it","it-IT","de","de-DE","es","es-ES"].includes(e)?"icudt_EFIGS.dat":["zh","ko","ja"].includes(t)?"icudt_CJK.dat":"icudt_no_CJK.dat"}(t)):n="icudt.dat";if(n)for(let e=0;e<o.length;e++){const t=o[e];if(t.virtualPath===n)return t.name}}return e.globalizationMode="invariant",null}(new Date).valueOf();const C=class{constructor(e){this.url=e}toString(){return this.url}};async function k(e,t){try{const o="function"==typeof globalThis.fetch;if(Se){const n=e.startsWith("file://");if(!n&&o)return globalThis.fetch(e,t||{credentials:"same-origin"});p||(h=Ne.require("url"),p=Ne.require("fs")),n&&(e=h.fileURLToPath(e));const r=await p.promises.readFile(e);return{ok:!0,headers:{length:0,get:()=>null},url:e,arrayBuffer:()=>r,json:()=>JSON.parse(r),text:()=>{throw new Error("NotImplementedException")}}}if(o)return globalThis.fetch(e,t||{credentials:"same-origin"});if("function"==typeof read)return{ok:!0,url:e,headers:{length:0,get:()=>null},arrayBuffer:()=>new Uint8Array(read(e,"binary")),json:()=>JSON.parse(read(e,"utf8")),text:()=>read(e,"utf8")}}catch(t){return{ok:!1,url:e,status:500,headers:{length:0,get:()=>null},statusText:"ERR28: "+t,arrayBuffer:()=>{throw t},json:()=>{throw t},text:()=>{throw t}}}throw new Error("No fetch implementation available")}function I(e){return"string"!=typeof e&&Be(!1,"url must be a string"),!M(e)&&0!==e.indexOf("./")&&0!==e.indexOf("../")&&globalThis.URL&&globalThis.document&&globalThis.document.baseURI&&(e=new URL(e,globalThis.document.baseURI).toString()),e}const U=/^[a-zA-Z][a-zA-Z\d+\-.]*?:\/\//,P=/[a-zA-Z]:[\\/]/;function M(e){return Se||Ie?e.startsWith("/")||e.startsWith("\\")||-1!==e.indexOf("///")||P.test(e):U.test(e)}let L,N=0;const $=[],z=[],W=new Map,F={"js-module-threads":!0,"js-module-runtime":!0,"js-module-dotnet":!0,"js-module-native":!0,"js-module-diagnostics":!0},B={...F,"js-module-library-initializer":!0},V={...F,dotnetwasm:!0,heap:!0,manifest:!0},q={...B,manifest:!0},H={...B,dotnetwasm:!0},J={dotnetwasm:!0,symbols:!0},Z={...B,dotnetwasm:!0,symbols:!0},Q={symbols:!0};function G(e){return!("icu"==e.behavior&&e.name!=Pe.preferredIcuAsset)}function K(e,t,o){null!=t||(t=[]),Be(1==t.length,`Expect to have one ${o} asset in resources`);const n=t[0];return n.behavior=o,X(n),e.push(n),n}function X(e){V[e.behavior]&&W.set(e.behavior,e)}function Y(e){Be(V[e],`Unknown single asset behavior ${e}`);const t=W.get(e);if(t&&!t.resolvedUrl)if(t.resolvedUrl=Pe.locateFile(t.name),F[t.behavior]){const e=ge(t);e?("string"!=typeof e&&Be(!1,"loadBootResource response for 'dotnetjs' type should be a URL string"),t.resolvedUrl=e):t.resolvedUrl=ce(t.resolvedUrl,t.behavior)}else if("dotnetwasm"!==t.behavior)throw new Error(`Unknown single asset behavior ${e}`);return t}function ee(e){const t=Y(e);return Be(t,`Single asset for ${e} not found`),t}let te=!1;async function oe(){if(!te){te=!0,Pe.diagnosticTracing&&b("mono_download_assets");try{const e=[],t=[],o=(e,t)=>{!Z[e.behavior]&&G(e)&&Pe.expected_instantiated_assets_count++,!H[e.behavior]&&G(e)&&(Pe.expected_downloaded_assets_count++,t.push(se(e)))};for(const t of $)o(t,e);for(const e of z)o(e,t);Pe.allDownloadsQueued.promise_control.resolve(),Promise.all([...e,...t]).then((()=>{Pe.allDownloadsFinished.promise_control.resolve()})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e})),await Pe.runtimeModuleLoaded.promise;const n=async e=>{const t=await e;if(t.buffer){if(!Z[t.behavior]){t.buffer&&"object"==typeof t.buffer||Be(!1,"asset buffer must be array-like or buffer-like or promise of these"),"string"!=typeof t.resolvedUrl&&Be(!1,"resolvedUrl must be string");const e=t.resolvedUrl,o=await t.buffer,n=new Uint8Array(o);pe(t),await Ue.beforeOnRuntimeInitialized.promise,Ue.instantiate_asset(t,e,n)}}else J[t.behavior]?("symbols"===t.behavior&&(await Ue.instantiate_symbols_asset(t),pe(t)),J[t.behavior]&&++Pe.actual_downloaded_assets_count):(t.isOptional||Be(!1,"Expected asset to have the downloaded buffer"),!H[t.behavior]&&G(t)&&Pe.expected_downloaded_assets_count--,!Z[t.behavior]&&G(t)&&Pe.expected_instantiated_assets_count--)},r=[],i=[];for(const t of e)r.push(n(t));for(const e of t)i.push(n(e));Promise.all(r).then((()=>{Ce||Ue.coreAssetsInMemory.promise_control.resolve()})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e})),Promise.all(i).then((async()=>{Ce||(await Ue.coreAssetsInMemory.promise,Ue.allAssetsInMemory.promise_control.resolve())})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e}))}catch(e){throw Pe.err("Error in mono_download_assets: "+e),e}}}let ne=!1;function re(){if(ne)return;ne=!0;const e=Pe.config,t=[];if(e.assets)for(const t of e.assets)"object"!=typeof t&&Be(!1,`asset must be object, it was ${typeof t} : ${t}`),"string"!=typeof t.behavior&&Be(!1,"asset behavior must be known string"),"string"!=typeof t.name&&Be(!1,"asset name must be string"),t.resolvedUrl&&"string"!=typeof t.resolvedUrl&&Be(!1,"asset resolvedUrl could be string"),t.hash&&"string"!=typeof t.hash&&Be(!1,"asset resolvedUrl could be string"),t.pendingDownload&&"object"!=typeof t.pendingDownload&&Be(!1,"asset pendingDownload could be object"),t.isCore?$.push(t):z.push(t),X(t);else if(e.resources){const o=e.resources;o.wasmNative||Be(!1,"resources.wasmNative must be defined"),o.jsModuleNative||Be(!1,"resources.jsModuleNative must be defined"),o.jsModuleRuntime||Be(!1,"resources.jsModuleRuntime must be defined"),K(z,o.wasmNative,"dotnetwasm"),K(t,o.jsModuleNative,"js-module-native"),K(t,o.jsModuleRuntime,"js-module-runtime"),o.jsModuleDiagnostics&&K(t,o.jsModuleDiagnostics,"js-module-diagnostics");const n=(e,t,o)=>{const n=e;n.behavior=t,o?(n.isCore=!0,$.push(n)):z.push(n)};if(o.coreAssembly)for(let e=0;e<o.coreAssembly.length;e++)n(o.coreAssembly[e],"assembly",!0);if(o.assembly)for(let e=0;e<o.assembly.length;e++)n(o.assembly[e],"assembly",!o.coreAssembly);if(0!=e.debugLevel&&Pe.isDebuggingSupported()){if(o.corePdb)for(let e=0;e<o.corePdb.length;e++)n(o.corePdb[e],"pdb",!0);if(o.pdb)for(let e=0;e<o.pdb.length;e++)n(o.pdb[e],"pdb",!o.corePdb)}if(e.loadAllSatelliteResources&&o.satelliteResources)for(const e in o.satelliteResources)for(let t=0;t<o.satelliteResources[e].length;t++){const r=o.satelliteResources[e][t];r.culture=e,n(r,"resource",!o.coreAssembly)}if(o.coreVfs)for(let e=0;e<o.coreVfs.length;e++)n(o.coreVfs[e],"vfs",!0);if(o.vfs)for(let e=0;e<o.vfs.length;e++)n(o.vfs[e],"vfs",!o.coreVfs);const r=O(e);if(r&&o.icu)for(let e=0;e<o.icu.length;e++){const t=o.icu[e];t.name===r&&n(t,"icu",!1)}if(o.wasmSymbols)for(let e=0;e<o.wasmSymbols.length;e++)n(o.wasmSymbols[e],"symbols",!1)}if(e.appsettings)for(let t=0;t<e.appsettings.length;t++){const o=e.appsettings[t],n=he(o);"appsettings.json"!==n&&n!==`appsettings.${e.applicationEnvironment}.json`||z.push({name:o,behavior:"vfs",cache:"no-cache",useCredentials:!0})}e.assets=[...$,...z,...t]}async function ie(e){const t=await se(e);return await t.pendingDownloadInternal.response,t.buffer}async function se(e){try{return await ae(e)}catch(t){if(!Pe.enableDownloadRetry)throw t;if(Ie||Se)throw t;if(e.pendingDownload&&e.pendingDownloadInternal==e.pendingDownload)throw t;if(e.resolvedUrl&&-1!=e.resolvedUrl.indexOf("file://"))throw t;if(t&&404==t.status)throw t;e.pendingDownloadInternal=void 0,await Pe.allDownloadsQueued.promise;try{return Pe.diagnosticTracing&&b(`Retrying download '${e.name}'`),await ae(e)}catch(t){return e.pendingDownloadInternal=void 0,await new Promise((e=>globalThis.setTimeout(e,100))),Pe.diagnosticTracing&&b(`Retrying download (2) '${e.name}' after delay`),await ae(e)}}}async function ae(e){for(;L;)await L.promise;try{++N,N==Pe.maxParallelDownloads&&(Pe.diagnosticTracing&&b("Throttling further parallel downloads"),L=i());const t=await async function(e){if(e.pendingDownload&&(e.pendingDownloadInternal=e.pendingDownload),e.pendingDownloadInternal&&e.pendingDownloadInternal.response)return e.pendingDownloadInternal.response;if(e.buffer){const t=await e.buffer;return e.resolvedUrl||(e.resolvedUrl="undefined://"+e.name),e.pendingDownloadInternal={url:e.resolvedUrl,name:e.name,response:Promise.resolve({ok:!0,arrayBuffer:()=>t,json:()=>JSON.parse(new TextDecoder("utf-8").decode(t)),text:()=>{throw new Error("NotImplementedException")},headers:{get:()=>{}}})},e.pendingDownloadInternal.response}const t=e.loadRemote&&Pe.config.remoteSources?Pe.config.remoteSources:[""];let o;for(let n of t){n=n.trim(),"./"===n&&(n="");const t=le(e,n);e.name===t?Pe.diagnosticTracing&&b(`Attempting to download '${t}'`):Pe.diagnosticTracing&&b(`Attempting to download '${t}' for ${e.name}`);try{e.resolvedUrl=t;const n=fe(e);if(e.pendingDownloadInternal=n,o=await n.response,!o||!o.ok)continue;return o}catch(e){o||(o={ok:!1,url:t,status:0,statusText:""+e});continue}}const n=e.isOptional||e.name.match(/\.pdb$/)&&Pe.config.ignorePdbLoadErrors;if(o||Be(!1,`Response undefined ${e.name}`),!n){const t=new Error(`download '${o.url}' for ${e.name} failed ${o.status} ${o.statusText}`);throw t.status=o.status,t}y(`optional download '${o.url}' for ${e.name} failed ${o.status} ${o.statusText}`)}(e);return t?(J[e.behavior]||(e.buffer=await t.arrayBuffer(),++Pe.actual_downloaded_assets_count),e):e}finally{if(--N,L&&N==Pe.maxParallelDownloads-1){Pe.diagnosticTracing&&b("Resuming more parallel downloads");const e=L;L=void 0,e.promise_control.resolve()}}}function le(e,t){let o;return null==t&&Be(!1,`sourcePrefix must be provided for ${e.name}`),e.resolvedUrl?o=e.resolvedUrl:(o=""===t?"assembly"===e.behavior||"pdb"===e.behavior?e.name:"resource"===e.behavior&&e.culture&&""!==e.culture?`${e.culture}/${e.name}`:e.name:t+e.name,o=ce(Pe.locateFile(o),e.behavior)),o&&"string"==typeof o||Be(!1,"attemptUrl need to be path or url string"),o}function ce(e,t){return Pe.modulesUniqueQuery&&q[t]&&(e+=Pe.modulesUniqueQuery),e}let de=0;const ue=new Set;function fe(e){try{e.resolvedUrl||Be(!1,"Request's resolvedUrl must be set");const t=function(e){let t=e.resolvedUrl;if(Pe.loadBootResource){const o=ge(e);if(o instanceof Promise)return o;"string"==typeof o&&(t=o)}const o={};return e.cache?o.cache=e.cache:Pe.config.disableNoCacheFetch||(o.cache="no-cache"),e.useCredentials?o.credentials="include":!Pe.config.disableIntegrityCheck&&e.hash&&(o.integrity=e.hash),Pe.fetch_like(t,o)}(e),o={name:e.name,url:e.resolvedUrl,response:t};return ue.add(e.name),o.response.then((()=>{"assembly"==e.behavior&&Pe.loadedAssemblies.push(e.name),de++,Pe.onDownloadResourceProgress&&Pe.onDownloadResourceProgress(de,ue.size)})),o}catch(t){const o={ok:!1,url:e.resolvedUrl,status:500,statusText:"ERR29: "+t,arrayBuffer:()=>{throw t},json:()=>{throw t}};return{name:e.name,url:e.resolvedUrl,response:Promise.resolve(o)}}}const me={resource:"assembly",assembly:"assembly",pdb:"pdb",icu:"globalization",vfs:"configuration",manifest:"manifest",dotnetwasm:"dotnetwasm","js-module-dotnet":"dotnetjs","js-module-native":"dotnetjs","js-module-runtime":"dotnetjs","js-module-threads":"dotnetjs"};function ge(e){var t;if(Pe.loadBootResource){const o=null!==(t=e.hash)&&void 0!==t?t:"",n=e.resolvedUrl,r=me[e.behavior];if(r){const t=Pe.loadBootResource(r,e.name,n,o,e.behavior);return"string"==typeof t?I(t):t}}}function pe(e){e.pendingDownloadInternal=null,e.pendingDownload=null,e.buffer=null,e.moduleExports=null}function he(e){let t=e.lastIndexOf("/");return t>=0&&t++,e.substring(t)}async function we(e){e&&await Promise.all((null!=e?e:[]).map((e=>async function(e){try{const t=e.name;if(!e.moduleExports){const o=ce(Pe.locateFile(t),"js-module-library-initializer");Pe.diagnosticTracing&&b(`Attempting to import '${o}' for ${e}`),e.moduleExports=await import(/*! webpackIgnore: true */o)}Pe.libraryInitializers.push({scriptName:t,exports:e.moduleExports})}catch(t){E(`Failed to import library initializer '${e}': ${t}`)}}(e))))}async function be(e,t){if(!Pe.libraryInitializers)return;const o=[];for(let n=0;n<Pe.libraryInitializers.length;n++){const r=Pe.libraryInitializers[n];r.exports[e]&&o.push(ye(r.scriptName,e,(()=>r.exports[e](...t))))}await Promise.all(o)}async function ye(e,t,o){try{await o()}catch(o){throw E(`Failed to invoke '${t}' on library initializer '${e}': ${o}`),Xe(1,o),o}}function ve(e,t){if(e===t)return e;const o={...t};return void 0!==o.assets&&o.assets!==e.assets&&(o.assets=[...e.assets||[],...o.assets||[]]),void 0!==o.resources&&(o.resources=_e(e.resources||{assembly:[],jsModuleNative:[],jsModuleRuntime:[],wasmNative:[]},o.resources)),void 0!==o.environmentVariables&&(o.environmentVariables={...e.environmentVariables||{},...o.environmentVariables||{}}),void 0!==o.runtimeOptions&&o.runtimeOptions!==e.runtimeOptions&&(o.runtimeOptions=[...e.runtimeOptions||[],...o.runtimeOptions||[]]),Object.assign(e,o)}function Ee(e,t){if(e===t)return e;const o={...t};return o.config&&(e.config||(e.config={}),o.config=ve(e.config,o.config)),Object.assign(e,o)}function _e(e,t){if(e===t)return e;const o={...t};return void 0!==o.coreAssembly&&(o.coreAssembly=[...e.coreAssembly||[],...o.coreAssembly||[]]),void 0!==o.assembly&&(o.assembly=[...e.assembly||[],...o.assembly||[]]),void 0!==o.lazyAssembly&&(o.lazyAssembly=[...e.lazyAssembly||[],...o.lazyAssembly||[]]),void 0!==o.corePdb&&(o.corePdb=[...e.corePdb||[],...o.corePdb||[]]),void 0!==o.pdb&&(o.pdb=[...e.pdb||[],...o.pdb||[]]),void 0!==o.jsModuleWorker&&(o.jsModuleWorker=[...e.jsModuleWorker||[],...o.jsModuleWorker||[]]),void 0!==o.jsModuleNative&&(o.jsModuleNative=[...e.jsModuleNative||[],...o.jsModuleNative||[]]),void 0!==o.jsModuleDiagnostics&&(o.jsModuleDiagnostics=[...e.jsModuleDiagnostics||[],...o.jsModuleDiagnostics||[]]),void 0!==o.jsModuleRuntime&&(o.jsModuleRuntime=[...e.jsModuleRuntime||[],...o.jsModuleRuntime||[]]),void 0!==o.wasmSymbols&&(o.wasmSymbols=[...e.wasmSymbols||[],...o.wasmSymbols||[]]),void 0!==o.wasmNative&&(o.wasmNative=[...e.wasmNative||[],...o.wasmNative||[]]),void 0!==o.icu&&(o.icu=[...e.icu||[],...o.icu||[]]),void 0!==o.satelliteResources&&(o.satelliteResources=function(e,t){if(e===t)return e;for(const o in t)e[o]=[...e[o]||[],...t[o]||[]];return e}(e.satelliteResources||{},o.satelliteResources||{})),void 0!==o.modulesAfterConfigLoaded&&(o.modulesAfterConfigLoaded=[...e.modulesAfterConfigLoaded||[],...o.modulesAfterConfigLoaded||[]]),void 0!==o.modulesAfterRuntimeReady&&(o.modulesAfterRuntimeReady=[...e.modulesAfterRuntimeReady||[],...o.modulesAfterRuntimeReady||[]]),void 0!==o.extensions&&(o.extensions={...e.extensions||{},...o.extensions||{}}),void 0!==o.vfs&&(o.vfs=[...e.vfs||[],...o.vfs||[]]),Object.assign(e,o)}function xe(){const e=Pe.config;if(e.environmentVariables=e.environmentVariables||{},e.runtimeOptions=e.runtimeOptions||[],e.resources=e.resources||{assembly:[],jsModuleNative:[],jsModuleWorker:[],jsModuleRuntime:[],wasmNative:[],vfs:[],satelliteResources:{}},e.assets){Pe.diagnosticTracing&&b("config.assets is deprecated, use config.resources instead");for(const t of e.assets){const o={};switch(t.behavior){case"assembly":o.assembly=[t];break;case"pdb":o.pdb=[t];break;case"resource":o.satelliteResources={},o.satelliteResources[t.culture]=[t];break;case"icu":o.icu=[t];break;case"symbols":o.wasmSymbols=[t];break;case"vfs":o.vfs=[t];break;case"dotnetwasm":o.wasmNative=[t];break;case"js-module-threads":o.jsModuleWorker=[t];break;case"js-module-runtime":o.jsModuleRuntime=[t];break;case"js-module-native":o.jsModuleNative=[t];break;case"js-module-diagnostics":o.jsModuleDiagnostics=[t];break;case"js-module-dotnet":break;default:throw new Error(`Unexpected behavior ${t.behavior} of asset ${t.name}`)}_e(e.resources,o)}}e.debugLevel,e.applicationEnvironment||(e.applicationEnvironment="Production"),e.applicationCulture&&(e.environmentVariables.LANG=`${e.applicationCulture}.UTF-8`),Ue.diagnosticTracing=Pe.diagnosticTracing=!!e.diagnosticTracing,Ue.waitForDebugger=e.waitForDebugger,Pe.maxParallelDownloads=e.maxParallelDownloads||Pe.maxParallelDownloads,Pe.enableDownloadRetry=void 0!==e.enableDownloadRetry?e.enableDownloadRetry:Pe.enableDownloadRetry}let je=!1;async function Re(e){var t;if(je)return void await Pe.afterConfigLoaded.promise;let o;try{if(e.configSrc||Pe.config&&0!==Object.keys(Pe.config).length&&(Pe.config.assets||Pe.config.resources)||(e.configSrc="dotnet.boot.js"),o=e.configSrc,je=!0,o&&(Pe.diagnosticTracing&&b("mono_wasm_load_config"),await async function(e){const t=e.configSrc,o=Pe.locateFile(t);let n=null;void 0!==Pe.loadBootResource&&(n=Pe.loadBootResource("manifest",t,o,"","manifest"));let r,i=null;if(n)if("string"==typeof n)n.includes(".json")?(i=await s(I(n)),r=await Ae(i)):r=(await import(I(n))).config;else{const e=await n;"function"==typeof e.json?(i=e,r=await Ae(i)):r=e.config}else o.includes(".json")?(i=await s(ce(o,"manifest")),r=await Ae(i)):r=(await import(ce(o,"manifest"))).config;function s(e){return Pe.fetch_like(e,{method:"GET",credentials:"include",cache:"no-cache"})}Pe.config.applicationEnvironment&&(r.applicationEnvironment=Pe.config.applicationEnvironment),ve(Pe.config,r)}(e)),xe(),await we(null===(t=Pe.config.resources)||void 0===t?void 0:t.modulesAfterConfigLoaded),await be("onRuntimeConfigLoaded",[Pe.config]),e.onConfigLoaded)try{await e.onConfigLoaded(Pe.config,Le),xe()}catch(e){throw _("onConfigLoaded() failed",e),e}xe(),Pe.afterConfigLoaded.promise_control.resolve(Pe.config)}catch(t){const n=`Failed to load config file ${o} ${t} ${null==t?void 0:t.stack}`;throw Pe.config=e.config=Object.assign(Pe.config,{message:n,error:t,isError:!0}),Xe(1,new Error(n)),t}}function Te(){return!!globalThis.navigator&&(Pe.isChromium||Pe.isFirefox)}async function Ae(e){const t=Pe.config,o=await e.json();t.applicationEnvironment||o.applicationEnvironment||(o.applicationEnvironment=e.headers.get("Blazor-Environment")||e.headers.get("DotNet-Environment")||void 0),o.environmentVariables||(o.environmentVariables={});const n=e.headers.get("DOTNET-MODIFIABLE-ASSEMBLIES");n&&(o.environmentVariables.DOTNET_MODIFIABLE_ASSEMBLIES=n);const r=e.headers.get("ASPNETCORE-BROWSER-TOOLS");return r&&(o.environmentVariables.__ASPNETCORE_BROWSER_TOOLS=r),o}"function"!=typeof importScripts||globalThis.onmessage||(globalThis.dotnetSidecar=!0);const Se="object"==typeof process&&"object"==typeof process.versions&&"string"==typeof process.versions.node,De="function"==typeof importScripts,Oe=De&&"undefined"!=typeof dotnetSidecar,Ce=De&&!Oe,ke="object"==typeof window||De&&!Se,Ie=!ke&&!Se;let Ue={},Pe={},Me={},Le={},Ne={},$e=!1;const ze={},We={config:ze},Fe={mono:{},binding:{},internal:Ne,module:We,loaderHelpers:Pe,runtimeHelpers:Ue,diagnosticHelpers:Me,api:Le};function Be(e,t){if(e)return;const o="Assert failed: "+("function"==typeof t?t():t),n=new Error(o);_(o,n),Ue.nativeAbort(n)}function Ve(){return void 0!==Pe.exitCode}function qe(){return Ue.runtimeReady&&!Ve()}function He(){Ve()&&Be(!1,`.NET runtime already exited with ${Pe.exitCode} ${Pe.exitReason}. You can use runtime.runMain() which doesn't exit the runtime.`),Ue.runtimeReady||Be(!1,".NET runtime didn't start yet. Please call dotnet.create() first.")}function Je(){ke&&(globalThis.addEventListener("unhandledrejection",et),globalThis.addEventListener("error",tt))}let Ze,Qe;function Ge(e){Qe&&Qe(e),Xe(e,Pe.exitReason)}function Ke(e){Ze&&Ze(e||Pe.exitReason),Xe(1,e||Pe.exitReason)}function Xe(t,o){var n,r;const i=o&&"object"==typeof o;t=i&&"number"==typeof o.status?o.status:void 0===t?-1:t;const s=i&&"string"==typeof o.message?o.message:""+o;(o=i?o:Ue.ExitStatus?function(e,t){const o=new Ue.ExitStatus(e);return o.message=t,o.toString=()=>t,o}(t,s):new Error("Exit with code "+t+" "+s)).status=t,o.message||(o.message=s);const a=""+(o.stack||(new Error).stack);try{Object.defineProperty(o,"stack",{get:()=>a})}catch(e){}const l=!!o.silent;if(o.silent=!0,Ve())Pe.diagnosticTracing&&b("mono_exit called after exit");else{try{We.onAbort==Ke&&(We.onAbort=Ze),We.onExit==Ge&&(We.onExit=Qe),ke&&(globalThis.removeEventListener("unhandledrejection",et),globalThis.removeEventListener("error",tt)),Ue.runtimeReady?(Ue.jiterpreter_dump_stats&&Ue.jiterpreter_dump_stats(!1),0===t&&(null===(n=Pe.config)||void 0===n?void 0:n.interopCleanupOnExit)&&Ue.forceDisposeProxies(!0,!0),e&&0!==t&&(null===(r=Pe.config)||void 0===r||r.dumpThreadsOnNonZeroExit)):(Pe.diagnosticTracing&&b(`abort_startup, reason: ${o}`),function(e){Pe.allDownloadsQueued.promise_control.reject(e),Pe.allDownloadsFinished.promise_control.reject(e),Pe.afterConfigLoaded.promise_control.reject(e),Pe.wasmCompilePromise.promise_control.reject(e),Pe.runtimeModuleLoaded.promise_control.reject(e),Ue.dotnetReady&&(Ue.dotnetReady.promise_control.reject(e),Ue.afterInstantiateWasm.promise_control.reject(e),Ue.beforePreInit.promise_control.reject(e),Ue.afterPreInit.promise_control.reject(e),Ue.afterPreRun.promise_control.reject(e),Ue.beforeOnRuntimeInitialized.promise_control.reject(e),Ue.afterOnRuntimeInitialized.promise_control.reject(e),Ue.afterPostRun.promise_control.reject(e))}(o))}catch(e){E("mono_exit A failed",e)}try{l||(function(e,t){if(0!==e&&t){const e=Ue.ExitStatus&&t instanceof Ue.ExitStatus?b:_;"string"==typeof t?e(t):(void 0===t.stack&&(t.stack=(new Error).stack+""),t.message?e(Ue.stringify_as_error_with_stack?Ue.stringify_as_error_with_stack(t.message+"\n"+t.stack):t.message+"\n"+t.stack):e(JSON.stringify(t)))}!Ce&&Pe.config&&(Pe.config.logExitCode?Pe.config.forwardConsoleLogsToWS?R("WASM EXIT "+e):v("WASM EXIT "+e):Pe.config.forwardConsoleLogsToWS&&R())}(t,o),function(e){if(ke&&!Ce&&Pe.config&&Pe.config.appendElementOnExit&&document){const t=document.createElement("label");t.id="tests_done",0!==e&&(t.style.background="red"),t.innerHTML=""+e,document.body.appendChild(t)}}(t))}catch(e){E("mono_exit B failed",e)}Pe.exitCode=t,Pe.exitReason||(Pe.exitReason=o),!Ce&&Ue.runtimeReady&&We.runtimeKeepalivePop()}if(Pe.config&&Pe.config.asyncFlushOnExit&&0===t)throw(async()=>{try{await async function(){try{const e=await import(/*! webpackIgnore: true */"process"),t=e=>new Promise(((t,o)=>{e.on("error",o),e.end("","utf8",t)})),o=t(e.stderr),n=t(e.stdout);let r;const i=new Promise((e=>{r=setTimeout((()=>e("timeout")),1e3)}));await Promise.race([Promise.all([n,o]),i]),clearTimeout(r)}catch(e){_(`flushing std* streams failed: ${e}`)}}()}finally{Ye(t,o)}})(),o;Ye(t,o)}function Ye(e,t){if(Ue.runtimeReady&&Ue.nativeExit)try{Ue.nativeExit(e)}catch(e){!Ue.ExitStatus||e instanceof Ue.ExitStatus||E("set_exit_code_and_quit_now failed: "+e.toString())}if(0!==e||!ke)throw Se&&Ne.process?Ne.process.exit(e):Ue.quit&&Ue.quit(e,t),t}function et(e){ot(e,e.reason,"rejection")}function tt(e){ot(e,e.error,"error")}function ot(e,t,o){e.preventDefault();try{t||(t=new Error("Unhandled "+o)),void 0===t.stack&&(t.stack=(new Error).stack),t.stack=t.stack+"",t.silent||(_("Unhandled error:",t),Xe(1,t))}catch(e){}}!function(e){if($e)throw new Error("Loader module already loaded");$e=!0,Ue=e.runtimeHelpers,Pe=e.loaderHelpers,Me=e.diagnosticHelpers,Le=e.api,Ne=e.internal,Object.assign(Le,{INTERNAL:Ne,invokeLibraryInitializers:be}),Object.assign(e.module,{config:ve(ze,{environmentVariables:{}})});const r={mono_wasm_bindings_is_ready:!1,config:e.module.config,diagnosticTracing:!1,nativeAbort:e=>{throw e||new Error("abort")},nativeExit:e=>{throw new Error("exit:"+e)}},l={gitHash:"95017c711e6afc1085133d440e42b4bd78155701",config:e.module.config,diagnosticTracing:!1,maxParallelDownloads:16,enableDownloadRetry:!0,_loaded_files:[],loadedFiles:[],loadedAssemblies:[],libraryInitializers:[],workerNextNumber:1,actual_downloaded_assets_count:0,actual_instantiated_assets_count:0,expected_downloaded_assets_count:0,expected_instantiated_assets_count:0,afterConfigLoaded:i(),allDownloadsQueued:i(),allDownloadsFinished:i(),wasmCompilePromise:i(),runtimeModuleLoaded:i(),loadingWorkers:i(),is_exited:Ve,is_runtime_running:qe,assert_runtime_running:He,mono_exit:Xe,createPromiseController:i,getPromiseController:s,assertIsControllablePromise:a,mono_download_assets:oe,resolve_single_asset_path:ee,setup_proxy_console:j,set_thread_prefix:w,installUnhandledErrorHandler:Je,retrieve_asset_download:ie,invokeLibraryInitializers:be,isDebuggingSupported:Te,exceptions:t,simd:n,relaxedSimd:o};Object.assign(Ue,r),Object.assign(Pe,l)}(Fe);let nt,rt,it,st=!1,at=!1;async function lt(e){if(!at){if(at=!0,ke&&Pe.config.forwardConsoleLogsToWS&&void 0!==globalThis.WebSocket&&j("main",globalThis.console,globalThis.location.origin),We||Be(!1,"Null moduleConfig"),Pe.config||Be(!1,"Null moduleConfig.config"),"function"==typeof e){const t=e(Fe.api);if(t.ready)throw new Error("Module.ready couldn't be redefined.");Object.assign(We,t),Ee(We,t)}else{if("object"!=typeof e)throw new Error("Can't use moduleFactory callback of createDotnetRuntime function.");Ee(We,e)}await async function(e){if(Se){const e=await import(/*! webpackIgnore: true */"process"),t=14;if(e.versions.node.split(".")[0]<t)throw new Error(`NodeJS at '${e.execPath}' has too low version '${e.versions.node}', please use at least ${t}. See also https://aka.ms/dotnet-wasm-features`)}const t=/*! webpackIgnore: true */import.meta.url,o=t.indexOf("?");var n;if(o>0&&(Pe.modulesUniqueQuery=t.substring(o)),Pe.scriptUrl=t.replace(/\\/g,"/").replace(/[?#].*/,""),Pe.scriptDirectory=(n=Pe.scriptUrl).slice(0,n.lastIndexOf("/"))+"/",Pe.locateFile=e=>"URL"in globalThis&&globalThis.URL!==C?new URL(e,Pe.scriptDirectory).toString():M(e)?e:Pe.scriptDirectory+e,Pe.fetch_like=k,Pe.out=console.log,Pe.err=console.error,Pe.onDownloadResourceProgress=e.onDownloadResourceProgress,ke&&globalThis.navigator){const e=globalThis.navigator,t=e.userAgentData&&e.userAgentData.brands;t&&t.length>0?Pe.isChromium=t.some((e=>"Google Chrome"===e.brand||"Microsoft Edge"===e.brand||"Chromium"===e.brand)):e.userAgent&&(Pe.isChromium=e.userAgent.includes("Chrome"),Pe.isFirefox=e.userAgent.includes("Firefox"))}Ne.require=Se?await import(/*! webpackIgnore: true */"module").then((e=>e.createRequire(/*! webpackIgnore: true */import.meta.url))):Promise.resolve((()=>{throw new Error("require not supported")})),void 0===globalThis.URL&&(globalThis.URL=C)}(We)}}async function ct(e){return await lt(e),Ze=We.onAbort,Qe=We.onExit,We.onAbort=Ke,We.onExit=Ge,We.ENVIRONMENT_IS_PTHREAD?async function(){(function(){const e=new MessageChannel,t=e.port1,o=e.port2;t.addEventListener("message",(e=>{var n,r;n=JSON.parse(e.data.config),r=JSON.parse(e.data.monoThreadInfo),st?Pe.diagnosticTracing&&b("mono config already received"):(ve(Pe.config,n),Ue.monoThreadInfo=r,xe(),Pe.diagnosticTracing&&b("mono config received"),st=!0,Pe.afterConfigLoaded.promise_control.resolve(Pe.config),ke&&n.forwardConsoleLogsToWS&&void 0!==globalThis.WebSocket&&Pe.setup_proxy_console("worker-idle",console,globalThis.location.origin)),t.close(),o.close()}),{once:!0}),t.start(),self.postMessage({[l]:{monoCmd:"preload",port:o}},[o])})(),await Pe.afterConfigLoaded.promise,function(){const e=Pe.config;e.assets||Be(!1,"config.assets must be defined");for(const t of e.assets)X(t),Q[t.behavior]&&z.push(t)}(),setTimeout((async()=>{try{await oe()}catch(e){Xe(1,e)}}),0);const e=dt(),t=await Promise.all(e);return await ut(t),We}():async function(){var e;await Re(We),re();const t=dt();(async function(){try{const e=ee("dotnetwasm");await se(e),e&&e.pendingDownloadInternal&&e.pendingDownloadInternal.response||Be(!1,"Can't load dotnet.native.wasm");const t=await e.pendingDownloadInternal.response,o=t.headers&&t.headers.get?t.headers.get("Content-Type"):void 0;let n;if("function"==typeof WebAssembly.compileStreaming&&"application/wasm"===o)n=await WebAssembly.compileStreaming(t);else{ke&&"application/wasm"!==o&&E('WebAssembly resource does not have the expected content type "application/wasm", so falling back to slower ArrayBuffer instantiation.');const e=await t.arrayBuffer();Pe.diagnosticTracing&&b("instantiate_wasm_module buffered"),n=Ie?await Promise.resolve(new WebAssembly.Module(e)):await WebAssembly.compile(e)}e.pendingDownloadInternal=null,e.pendingDownload=null,e.buffer=null,e.moduleExports=null,Pe.wasmCompilePromise.promise_control.resolve(n)}catch(e){Pe.wasmCompilePromise.promise_control.reject(e)}})(),setTimeout((async()=>{try{D(),await oe()}catch(e){Xe(1,e)}}),0);const o=await Promise.all(t);return await ut(o),await Ue.dotnetReady.promise,await we(null===(e=Pe.config.resources)||void 0===e?void 0:e.modulesAfterRuntimeReady),await be("onRuntimeReady",[Fe.api]),Le}()}function dt(){const e=ee("js-module-runtime"),t=ee("js-module-native");if(nt&&rt)return[nt,rt,it];"object"==typeof e.moduleExports?nt=e.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${e.resolvedUrl}' for ${e.name}`),nt=import(/*! webpackIgnore: true */e.resolvedUrl)),"object"==typeof t.moduleExports?rt=t.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${t.resolvedUrl}' for ${t.name}`),rt=import(/*! webpackIgnore: true */t.resolvedUrl));const o=Y("js-module-diagnostics");return o&&("object"==typeof o.moduleExports?it=o.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${o.resolvedUrl}' for ${o.name}`),it=import(/*! webpackIgnore: true */o.resolvedUrl))),[nt,rt,it]}async function ut(e){const{initializeExports:t,initializeReplacements:o,configureRuntimeStartup:n,configureEmscriptenStartup:r,configureWorkerStartup:i,setRuntimeGlobals:s,passEmscriptenInternals:a}=e[0],{default:l}=e[1],c=e[2];s(Fe),t(Fe),c&&c.setRuntimeGlobals(Fe),await n(We),Pe.runtimeModuleLoaded.promise_control.resolve(),l((e=>(Object.assign(We,{ready:e.ready,__dotnet_runtime:{initializeReplacements:o,configureEmscriptenStartup:r,configureWorkerStartup:i,passEmscriptenInternals:a}}),We))).catch((e=>{if(e.message&&e.message.toLowerCase().includes("out of memory"))throw new Error(".NET runtime has failed to start, because too much memory was requested. Please decrease the memory by adjusting EmccMaximumHeapSize. See also https://aka.ms/dotnet-wasm-features");throw e}))}const ft=new class{withModuleConfig(e){try{return Ee(We,e),this}catch(e){throw Xe(1,e),e}}withOnConfigLoaded(e){try{return Ee(We,{onConfigLoaded:e}),this}catch(e){throw Xe(1,e),e}}withConsoleForwarding(){try{return ve(ze,{forwardConsoleLogsToWS:!0}),this}catch(e){throw Xe(1,e),e}}withExitOnUnhandledError(){try{return ve(ze,{exitOnUnhandledError:!0}),Je(),this}catch(e){throw Xe(1,e),e}}withAsyncFlushOnExit(){try{return ve(ze,{asyncFlushOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withExitCodeLogging(){try{return ve(ze,{logExitCode:!0}),this}catch(e){throw Xe(1,e),e}}withElementOnExit(){try{return ve(ze,{appendElementOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withInteropCleanupOnExit(){try{return ve(ze,{interopCleanupOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withDumpThreadsOnNonZeroExit(){try{return ve(ze,{dumpThreadsOnNonZeroExit:!0}),this}catch(e){throw Xe(1,e),e}}withWaitingForDebugger(e){try{return ve(ze,{waitForDebugger:e}),this}catch(e){throw Xe(1,e),e}}withInterpreterPgo(e,t){try{return ve(ze,{interpreterPgo:e,interpreterPgoSaveDelay:t}),ze.runtimeOptions?ze.runtimeOptions.push("--interp-pgo-recording"):ze.runtimeOptions=["--interp-pgo-recording"],this}catch(e){throw Xe(1,e),e}}withConfig(e){try{return ve(ze,e),this}catch(e){throw Xe(1,e),e}}withConfigSrc(e){try{return e&&"string"==typeof e||Be(!1,"must be file path or URL"),Ee(We,{configSrc:e}),this}catch(e){throw Xe(1,e),e}}withVirtualWorkingDirectory(e){try{return e&&"string"==typeof e||Be(!1,"must be directory path"),ve(ze,{virtualWorkingDirectory:e}),this}catch(e){throw Xe(1,e),e}}withEnvironmentVariable(e,t){try{const o={};return o[e]=t,ve(ze,{environmentVariables:o}),this}catch(e){throw Xe(1,e),e}}withEnvironmentVariables(e){try{return e&&"object"==typeof e||Be(!1,"must be dictionary object"),ve(ze,{environmentVariables:e}),this}catch(e){throw Xe(1,e),e}}withDiagnosticTracing(e){try{return"boolean"!=typeof e&&Be(!1,"must be boolean"),ve(ze,{diagnosticTracing:e}),this}catch(e){throw Xe(1,e),e}}withDebugging(e){try{return null!=e&&"number"==typeof e||Be(!1,"must be number"),ve(ze,{debugLevel:e}),this}catch(e){throw Xe(1,e),e}}withApplicationArguments(...e){try{return e&&Array.isArray(e)||Be(!1,"must be array of strings"),ve(ze,{applicationArguments:e}),this}catch(e){throw Xe(1,e),e}}withRuntimeOptions(e){try{return e&&Array.isArray(e)||Be(!1,"must be array of strings"),ze.runtimeOptions?ze.runtimeOptions.push(...e):ze.runtimeOptions=e,this}catch(e){throw Xe(1,e),e}}withMainAssembly(e){try{return ve(ze,{mainAssemblyName:e}),this}catch(e){throw Xe(1,e),e}}withApplicationArgumentsFromQuery(){try{if(!globalThis.window)throw new Error("Missing window to the query parameters from");if(void 0===globalThis.URLSearchParams)throw new Error("URLSearchParams is supported");const e=new URLSearchParams(globalThis.window.location.search).getAll("arg");return this.withApplicationArguments(...e)}catch(e){throw Xe(1,e),e}}withApplicationEnvironment(e){try{return ve(ze,{applicationEnvironment:e}),this}catch(e){throw Xe(1,e),e}}withApplicationCulture(e){try{return ve(ze,{applicationCulture:e}),this}catch(e){throw Xe(1,e),e}}withResourceLoader(e){try{return Pe.loadBootResource=e,this}catch(e){throw Xe(1,e),e}}async download(){try{await async function(){lt(We),await Re(We),re(),D(),oe(),await Pe.allDownloadsFinished.promise}()}catch(e){throw Xe(1,e),e}}async create(){try{return this.instance||(this.instance=await async function(){return await ct(We),Fe.api}()),this.instance}catch(e){throw Xe(1,e),e}}async run(){try{return We.config||Be(!1,"Null moduleConfig.config"),this.instance||await this.create(),this.instance.runMainAndExit()}catch(e){throw Xe(1,e),e}}},mt=Xe,gt=ct;Ie||"function"==typeof globalThis.URL||Be(!1,"This browser/engine doesn't support URL API. Please use a modern version. See also https://aka.ms/dotnet-wasm-features"),"function"!=typeof globalThis.BigInt64Array&&Be(!1,"This browser/engine doesn't support BigInt64Array API. Please use a modern version. See also https://aka.ms/dotnet-wasm-features"),ft.withConfig(/*json-start*/{
  "mainAssemblyName": "Core2D.Browser",
  "resources": {
    "hash": "sha256-mBvSR+D0wYsqlPA5ENHkn8M/nm17cW2uwZZbjGaz7+Y=",
    "jsModuleNative": [
      {
        "name": "dotnet.native.m93zitftf0.js"
      }
    ],
    "jsModuleRuntime": [
      {
        "name": "dotnet.runtime.v06hirbjsv.js"
      }
    ],
    "wasmNative": [
      {
        "name": "dotnet.native.bufmemcc37.wasm",
        "hash": "sha256-s59vz/79Y411Aq7gJ7CyJfUHQ58d2yKkU3BBWHAVBOY=",
        "cache": "force-cache"
      }
    ],
    "icu": [
      {
        "virtualPath": "icudt_CJK.dat",
        "name": "icudt_CJK.tjcz0u77k5.dat",
        "hash": "sha256-SZLtQnRc0JkwqHab0VUVP7T3uBPSeYzxzDnpxPpUnHk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "icudt_EFIGS.dat",
        "name": "icudt_EFIGS.tptq2av103.dat",
        "hash": "sha256-8fItetYY8kQ0ww6oxwTLiT3oXlBwHKumbeP2pRF4yTc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "icudt_no_CJK.dat",
        "name": "icudt_no_CJK.lfu7j35m59.dat",
        "hash": "sha256-L7sV7NEYP37/Qr2FPCePo5cJqRgTXRwGHuwF5Q+0Nfs=",
        "cache": "force-cache"
      }
    ],
    "coreAssembly": [
      {
        "virtualPath": "System.Private.CoreLib.wasm",
        "name": "System.Private.CoreLib.o9rui6p1of.wasm",
        "hash": "sha256-jN8dujbWpY14rLL+IoYQ0OKwvEVm/++rqvx+7szOyjE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.InteropServices.JavaScript.wasm",
        "name": "System.Runtime.InteropServices.JavaScript.wmgbagosjd.wasm",
        "hash": "sha256-CM2d9Xzb1cqcRZrVIj7ldeY7/uQBjNwJZlzMOak/ku8=",
        "cache": "force-cache"
      }
    ],
    "assembly": [
      {
        "virtualPath": "ACadSharp.wasm",
        "name": "ACadSharp.oyfjj7pcei.wasm",
        "hash": "sha256-cClnxsMcc7nyJVW54ed03nzQmzbwHaMWmPC3xDwK2pg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Base.wasm",
        "name": "Avalonia.Base.gslero3l70.wasm",
        "hash": "sha256-MM8Sr9kzNIPxIGFetZt6VJoCpERDsBB16RYQ6KLk4Gs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Browser.wasm",
        "name": "Avalonia.Browser.fiuq9vxofd.wasm",
        "hash": "sha256-97GNHDRPF/JDVpe+TpUZkXihvffjwjOfBjRs1mf7liw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Controls.DataGrid.wasm",
        "name": "Avalonia.Controls.DataGrid.4tv5vboh5t.wasm",
        "hash": "sha256-uIT32revV7SCqSk/jp3soK3cXnq1m0i/WdRbxWJgqSU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Controls.wasm",
        "name": "Avalonia.Controls.0m7ifcqgec.wasm",
        "hash": "sha256-CLaGWsPI4P9lANrkblpQkW1HOLDX9iWbBH/8xGAxL44=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Desktop.wasm",
        "name": "Avalonia.Desktop.dsy5y3q3tm.wasm",
        "hash": "sha256-Xe3yZEV0Hc4Tjl84hPyI5F1K2T/6tCweHkvbubb/lOE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Dialogs.wasm",
        "name": "Avalonia.Dialogs.2mk90onnzs.wasm",
        "hash": "sha256-Zp+k/IRlnljoq2znU9YR6oS2z+aEjTujHh3aUg1gQoU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Fonts.Inter.wasm",
        "name": "Avalonia.Fonts.Inter.zgxm5hhx5b.wasm",
        "hash": "sha256-86sMOH7mMSxYJVPdTuIPbeX2bLRMOl/GyRGj8pkFQwE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Markup.Xaml.wasm",
        "name": "Avalonia.Markup.Xaml.lipgitp0e9.wasm",
        "hash": "sha256-LVvI19vZ0SISEli2Sxgxoz1NL3JoFfTThYOUITVUCL4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Markup.wasm",
        "name": "Avalonia.Markup.uh4142q83v.wasm",
        "hash": "sha256-/sZq3ce75+XpJyf3QKDvcM9UKZFetA6svl4GhaUY8vg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Metal.wasm",
        "name": "Avalonia.Metal.c2y0nr7ufw.wasm",
        "hash": "sha256-jKDu/O1HGruOKupF8+LINURmO1JF6sv9qEB48ResVjg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.OpenGL.wasm",
        "name": "Avalonia.OpenGL.v3mhpr6jho.wasm",
        "hash": "sha256-K6+mtz1yLZ8birzlyeLXlz8Xxt6rgjYDSctoj2Dm0tY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Skia.wasm",
        "name": "Avalonia.Skia.0phldy2be3.wasm",
        "hash": "sha256-LN70f14z6wmdKmpWsx/1W/8HdqsyZcHNdv8xwIB0Q4w=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Themes.Browser.wasm",
        "name": "Avalonia.Themes.Browser.g9v7as1jp7.wasm",
        "hash": "sha256-/Fs9XA1dzLu363ibsoAwVOU6q8QsaHg8MrM/csFs4p0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Vulkan.wasm",
        "name": "Avalonia.Vulkan.fik2ar6f1z.wasm",
        "hash": "sha256-V4r+yBRjbSJbhw0hfeJjMHJ0/WpnetdkXhgrPMhevSI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "AvaloniaEdit.TextMate.wasm",
        "name": "AvaloniaEdit.TextMate.gwqiyyuu0a.wasm",
        "hash": "sha256-UB2ag/aLWWyVQ7Vp+waYOUYRY8CcGbmp4S5APP0NzGI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "AvaloniaEdit.wasm",
        "name": "AvaloniaEdit.l25g627z2e.wasm",
        "hash": "sha256-rbuwMPLs3oU7XyF+j82VFyirl9E+lxTGE43T+nOPh4s=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "CommunityToolkit.Mvvm.wasm",
        "name": "CommunityToolkit.Mvvm.x91ymstd8h.wasm",
        "hash": "sha256-9RDAUDRwHuxp+/WD2Kk3kCWycihEmaMofN2NwEK61e4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.wasm",
        "name": "Core2D.d9z9rzx1pr.wasm",
        "hash": "sha256-Py6suDB8ysG9NZ77Ci5GEW7yvasdaOdTtygaWkuupNk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Browser.wasm",
        "name": "Core2D.Browser.xz1hy9jbni.wasm",
        "hash": "sha256-XTpNkA40RwZPPRQNySwmiKi90NdmekwjN1ZtlWCyNP0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Controls.Ruler.wasm",
        "name": "Core2D.Controls.Ruler.nkc8457ogf.wasm",
        "hash": "sha256-pxmtwuGHTTRMIndpGJnve97YTvyHRand/CPiSv2Ses0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.FileSystem.wasm",
        "name": "Core2D.Modules.FileSystem.ne1dlr67f2.wasm",
        "hash": "sha256-oRb43LNavQPDOlwNBKrLtdlnr1vI/vONXoBDqG6GB7Q=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.FileWriter.wasm",
        "name": "Core2D.Modules.FileWriter.nkd9i7s3ri.wasm",
        "hash": "sha256-CQVetVut4M2mLBKW7fdXTQZsVh855iwj1BsoJ+vLOUQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Log.wasm",
        "name": "Core2D.Modules.Log.hp2tx2jnvu.wasm",
        "hash": "sha256-C1RLBzU1cLmdT2+TvtiAb+bQfU8T8PmtAluTSCs1VbQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.Avalonia.wasm",
        "name": "Core2D.Modules.Renderer.Avalonia.3m5vexgifd.wasm",
        "hash": "sha256-K6CA1NtzxOcWc45EfOf1kyqDvUfeKCrMOiFWzWoHO9s=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.Common.wasm",
        "name": "Core2D.Modules.Renderer.Common.0k8elf35me.wasm",
        "hash": "sha256-MJzZrErFHA/e2wIMzEPZRcFnZ17MICfo8QwQsy08kDk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.Dwg.wasm",
        "name": "Core2D.Modules.Renderer.Dwg.6aegv9bn10.wasm",
        "hash": "sha256-sqDrsi85KFfn9suPIHzVLFvn2tua3kl0f84D/T+tbMM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.Dxf.wasm",
        "name": "Core2D.Modules.Renderer.Dxf.c5zj9wm7gd.wasm",
        "hash": "sha256-sIH8GFO+SIUtaqH4MmVgNjO5C4TuNGaeSEVEaIlPPjs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.OpenXml.wasm",
        "name": "Core2D.Modules.Renderer.OpenXml.iih417td92.wasm",
        "hash": "sha256-VqFZo2px9izHZ7PnwHTkB4zsr+T2bnEOV0VuwVbeMKM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.PdfSharp.wasm",
        "name": "Core2D.Modules.Renderer.PdfSharp.tnvhtnnccs.wasm",
        "hash": "sha256-d9y10H1nWsyo/lRQzwsdRYRMgo5OIobmjuToOv7RkPE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.SkiaSharp.wasm",
        "name": "Core2D.Modules.Renderer.SkiaSharp.zp3crqfl2v.wasm",
        "hash": "sha256-13AMK2o8/oMgHR7vizGQp82jj+u2GzY6bZ1C3RdOKNU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.SparseStrips.wasm",
        "name": "Core2D.Modules.Renderer.SparseStrips.lfara93w7w.wasm",
        "hash": "sha256-kaufZ4GO8wU6uvohkM5fwAXEfD5AStG3D89L//e5bMc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.VelloSharp.wasm",
        "name": "Core2D.Modules.Renderer.VelloSharp.ax7e0nkr9x.wasm",
        "hash": "sha256-ntsp11xF+mo/c+9JUHmz6nuVbVBn9e8Mw/quCjque1I=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.WinForms.wasm",
        "name": "Core2D.Modules.Renderer.WinForms.8d6v7zkrkr.wasm",
        "hash": "sha256-BPvzvnhhitH3UZ9ryrJoQQkhHBvVAKmoGso5Ia6ESyM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Renderer.Wmf.wasm",
        "name": "Core2D.Modules.Renderer.Wmf.8cr3e810ev.wasm",
        "hash": "sha256-KKs3f4YPWc14M3KIdSsrLhrM8yYr9oQ+WMT9+OJHNgU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.ScriptRunner.wasm",
        "name": "Core2D.Modules.ScriptRunner.is11578iej.wasm",
        "hash": "sha256-UmAuGTgPckgZGNQRNRChhIK4Cvw8tKfS5Iwx956lkJw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.Serializer.wasm",
        "name": "Core2D.Modules.Serializer.naz00nl11e.wasm",
        "hash": "sha256-UQGHE5QDkF2JL84o2UgTeLQvD81ri9hr3LB5KYMv8f0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.SvgExporter.wasm",
        "name": "Core2D.Modules.SvgExporter.j13x0az4kj.wasm",
        "hash": "sha256-pyXaoWqDwUcGdF1ZtTmrgJ/M7YZadBwL1UBR6/zBdbg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.TextFieldReader.wasm",
        "name": "Core2D.Modules.TextFieldReader.rch603pye5.wasm",
        "hash": "sha256-W5mztbJYGSV2Gqi5xUB5jaNS5FYWm0nS8KQmegbsVSI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.TextFieldWriter.wasm",
        "name": "Core2D.Modules.TextFieldWriter.ha03ptguw0.wasm",
        "hash": "sha256-xw4sSZEbCjPAtL11oRnY5bNqt4xM2tKTbexlEB4kNoY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Modules.XamlExporter.wasm",
        "name": "Core2D.Modules.XamlExporter.8oset9hbrq.wasm",
        "hash": "sha256-WKaurSi2XlC+yin2JEEyduYvpNuahflNQML9zCoN8Dk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.Spatial.wasm",
        "name": "Core2D.Spatial.90iw4pk8bh.wasm",
        "hash": "sha256-1xRqZ2a2ARLWBdSWt89bQuop8JibrhkjnjEi/FP0DDw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Core2D.ViewModels.wasm",
        "name": "Core2D.ViewModels.v5eatey5x1.wasm",
        "hash": "sha256-zQTgswSCVPGE9xt5uEW5fFlREHYzRCsnz//7u1b8uZ8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "CsvHelper.wasm",
        "name": "CsvHelper.uav9ykykb7.wasm",
        "hash": "sha256-Sqf4YjcJyrWzdR310OoO/RGro1VJVpNoCIHnq7zcDuc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Dock.Avalonia.wasm",
        "name": "Dock.Avalonia.wgchcbll2c.wasm",
        "hash": "sha256-YIHe2ppG3q9LSR2wzkNcbzQHpEtIQhzBCppVO5Iy7cw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Dock.Avalonia.Themes.Fluent.wasm",
        "name": "Dock.Avalonia.Themes.Fluent.ibckyaii3e.wasm",
        "hash": "sha256-XuXKXET/GVx0aIM9/MIdak4AT1snLXWz16cnJLxn2mw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Dock.Controls.ProportionalStackPanel.wasm",
        "name": "Dock.Controls.ProportionalStackPanel.v0gi1wiu91.wasm",
        "hash": "sha256-n+JiNMC3ZnIYQ2eFX8deIH8I+0/oSr+Sw+R8W5PEZbQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Dock.Controls.Recycling.Model.wasm",
        "name": "Dock.Controls.Recycling.Model.d2stxb8dm2.wasm",
        "hash": "sha256-qz94axDQI7u/0gSz7bb4yMb4AdbDYCg343KzXBcwqAU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Dock.Controls.Recycling.wasm",
        "name": "Dock.Controls.Recycling.medz4w422m.wasm",
        "hash": "sha256-Yg2J9xiPckqSSS836Fh0j3Yf804yRI+yawiTTDJBWQA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Dock.Model.ReactiveUI.wasm",
        "name": "Dock.Model.ReactiveUI.sq6ihitffr.wasm",
        "hash": "sha256-XESaFtQaJBzE1pxlWEdchDI+Fywm5zXqaKwvUvio4Hs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Dock.Model.wasm",
        "name": "Dock.Model.b6c6164ftr.wasm",
        "hash": "sha256-I/zIfBSUvc8JAP8RSu+Tk+qWwGeBVYN7V7FRVm0LD/E=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Dock.Settings.wasm",
        "name": "Dock.Settings.9ha84oof24.wasm",
        "hash": "sha256-8/Ti8WCTxvbQ0yegwh5YQdYPc26R5Z1Y5irYzEphDAA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "DocumentFormat.OpenXml.Framework.wasm",
        "name": "DocumentFormat.OpenXml.Framework.wmuqp1c6cp.wasm",
        "hash": "sha256-aEV3DoJRFV2jnvKxxdfbYtk6twxCNaLanuql1XARY9k=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "DocumentFormat.OpenXml.wasm",
        "name": "DocumentFormat.OpenXml.sa6a8hhg7h.wasm",
        "hash": "sha256-nMU3lVeKNGv6Ry1lmh52ifJQE8xvTZ6X/7ofDD/dCSo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "ExCSS.wasm",
        "name": "ExCSS.fb3gi43no9.wasm",
        "hash": "sha256-y0bE9LvHNoJAqCw89snlpKe9ZA68xXD504xPAGsDD9c=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "HarfBuzzSharp.wasm",
        "name": "HarfBuzzSharp.l6qemr74ut.wasm",
        "hash": "sha256-8/Et8AiCR9v3jIgVnztdiSU4jmQcE7FwRkuMyZPFJ7Y=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.CSharp.wasm",
        "name": "Microsoft.CSharp.kc7tjlkvly.wasm",
        "hash": "sha256-jM7wwSeJRczfk7Ix/T2DwFC4cg2lenKpBngI0mfGUjw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.wasm",
        "name": "Microsoft.CodeAnalysis.CSharp.Scripting.gmfdwhll1s.wasm",
        "hash": "sha256-WCRb0wx/UYYE2RIavgJHl4dtlAArIPVFfM5p3GoYbY0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.CSharp.wasm",
        "name": "Microsoft.CodeAnalysis.CSharp.cgulcbtgvb.wasm",
        "hash": "sha256-bPV7bPRhYDpzoYLpNzCg8pdzw2yolve3dTfuYAQX7e4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.Scripting.wasm",
        "name": "Microsoft.CodeAnalysis.Scripting.ptby2jtgz0.wasm",
        "hash": "sha256-Xa5xmMhe5540nFgP1Qs8tI8t5ZfSrp8c9xryOVUHgCU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.wasm",
        "name": "Microsoft.CodeAnalysis.dwbv5q1nxj.wasm",
        "hash": "sha256-/eqeYZVm3s4ggMZY6IjROYtgMpbthUeRUQjmcSWi8rI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyInjection.Abstractions.wasm",
        "name": "Microsoft.Extensions.DependencyInjection.Abstractions.7eq1f8zv3d.wasm",
        "hash": "sha256-37o2iMsHhoUmuxAvnG2RpmevNyQ+vjZw9HgboSK5QSU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyInjection.wasm",
        "name": "Microsoft.Extensions.DependencyInjection.qk0e9fmrwq.wasm",
        "hash": "sha256-lqDrHL4RQpPFr/VeP8zhoX9pzh6j7kldJ3N0UfGFp4k=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.Abstractions.wasm",
        "name": "Microsoft.Extensions.Logging.Abstractions.byb5dzsqe2.wasm",
        "hash": "sha256-w6QDRbYs0X8pHLLBQieB8HlviK0WuWISxClJU4NoAp8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Newtonsoft.Json.wasm",
        "name": "Newtonsoft.Json.1xphlk3t1t.wasm",
        "hash": "sha256-dRILD5yH0Sppp7VoW+5MTeQpgQuSLUtqadjsenQg0F4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Onigwrap.wasm",
        "name": "Onigwrap.vtapslsauz.wasm",
        "hash": "sha256-E1PN0uZBUDU0f+fu6Kh+rL6j44DKYZOapJmr+f8Ir88=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Oxage.Wmf.wasm",
        "name": "Oxage.Wmf.6idmwy9yph.wasm",
        "hash": "sha256-IhpZowMC9ruFfa/P4j+2KIqRn4ijou8hKDagT1EjvRQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "PanAndZoom.wasm",
        "name": "PanAndZoom.771fjvttc3.wasm",
        "hash": "sha256-LW0a39jkXYSkHb2ePwv9mwkKMyOpdC7n8Nqj8NbGAro=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "PdfSharp.wasm",
        "name": "PdfSharp.r9cri0ir7k.wasm",
        "hash": "sha256-IVcYAHGlSR3+iVWCsuYRN/msZOclQfp+f7hb3KhHD54=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "PdfSharp.Shared.wasm",
        "name": "PdfSharp.Shared.7nmrg9oyzd.wasm",
        "hash": "sha256-nYANGiGlAYnYDYo4bk6ePtuamIQ/6E2maMdMchEwGh0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "PdfSharp.System.wasm",
        "name": "PdfSharp.System.0ako4rx14f.wasm",
        "hash": "sha256-WGN5oBjfER2tSEyVbJO+dSgFhyfm60LocQ4ywbCPAmw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "ProDataGrid.FormulaEngine.Excel.wasm",
        "name": "ProDataGrid.FormulaEngine.Excel.cs6z67dfqe.wasm",
        "hash": "sha256-HN9YIUeLqY4ZUfHECw2X3gVZqNzKqwfiexVzI7s6JNg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "ProDataGrid.FormulaEngine.wasm",
        "name": "ProDataGrid.FormulaEngine.t4v6xwk41j.wasm",
        "hash": "sha256-HWTar+aPQic9yzBmqM4PTtKGd1xpS4DU54ShZUdsaqg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "ReactiveUI.wasm",
        "name": "ReactiveUI.7faqtu3dh3.wasm",
        "hash": "sha256-Q5zce142fNR8MrpPk4oIxOYdJSDKLaTneoPM7jZR5Zg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "ShimSkiaSharp.wasm",
        "name": "ShimSkiaSharp.ihpx9f1hy9.wasm",
        "hash": "sha256-umsK0G0C4WyrjlkKP6XPHi3k8t/CscC06kbr4j2RdPU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "SkiaSharp.wasm",
        "name": "SkiaSharp.9c42tq86o4.wasm",
        "hash": "sha256-h5i8DYfoH0v5xG6hDdtAm5AXk+T2xDijWCPpbq1sHd4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Splat.wasm",
        "name": "Splat.e8equ8zey3.wasm",
        "hash": "sha256-jngd7cDQb1FDmvRhxYGJPwsXIfTs9M9xjTqNs4vd/sk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Splat.Builder.wasm",
        "name": "Splat.Builder.r4641hn12j.wasm",
        "hash": "sha256-Cy2o/T9druABy/wJcmwAiapXPQkoBCY3yo7nfyU54oc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Splat.Core.wasm",
        "name": "Splat.Core.glthxrc83f.wasm",
        "hash": "sha256-7IJwjvxyMobeuy/fIs3m5y4BU0I+TrN6Bwdj84KJMsY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Splat.Logging.wasm",
        "name": "Splat.Logging.0gircqxpuf.wasm",
        "hash": "sha256-4OWEtDhFoImkJaLUx7tDem3zHMzxUxz3f5fGCVieXL4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.Custom.wasm",
        "name": "Svg.Custom.uykpzxxybi.wasm",
        "hash": "sha256-mhKFd5+RIdfJ6ahcR0aYqJ/xrPqYXenorZTFcgOg2HE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.Model.wasm",
        "name": "Svg.Model.l3uy3ej28q.wasm",
        "hash": "sha256-4Vclsoywv5NznxGV908sq1iDX7zSZ2hR7WtjQMJPTBw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.Skia.wasm",
        "name": "Svg.Skia.l0m1k8wc0b.wasm",
        "hash": "sha256-tdkfb5G2Ny0akH2/QlrjVKupQQI7L+HiAN6WZOn0hug=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.Concurrent.wasm",
        "name": "System.Collections.Concurrent.998g67hpnx.wasm",
        "hash": "sha256-M+AAcQ52Eq6QzqYSJ0Xk2+Caj+cOjpQhLND2SOS5vN8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.Immutable.wasm",
        "name": "System.Collections.Immutable.vxe21duqtz.wasm",
        "hash": "sha256-3fIqDOSq2L7juM0HDxypKWbYPwE77G7DaH/fmExJtMk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.NonGeneric.wasm",
        "name": "System.Collections.NonGeneric.2g2468i6rl.wasm",
        "hash": "sha256-npRKGVTaNkCwN0Te8NOzI21u57mXTYVS6S78DTOxAnM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.Specialized.wasm",
        "name": "System.Collections.Specialized.h8nd2pxbbq.wasm",
        "hash": "sha256-4cz3BtfYJzNo2wHb6D/Tld/tU32c2WC/YYdRqzPHCzo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.wasm",
        "name": "System.Collections.v9xoev5486.wasm",
        "hash": "sha256-c7jmRRqqX4Jrr3plY2cR6G9r3oVLaHmIIUipPa1WidI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.Annotations.wasm",
        "name": "System.ComponentModel.Annotations.51e63h85cp.wasm",
        "hash": "sha256-iRNWs0Mgh7Mg7V9di9MdH9J3jn5s5Es/DEPOX1VLUZ4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.Primitives.wasm",
        "name": "System.ComponentModel.Primitives.p3osq0bk8o.wasm",
        "hash": "sha256-+4x2hdAeqguvkgqR2QH34gRjY+ynb4y1vXtYX6sbRBQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.TypeConverter.wasm",
        "name": "System.ComponentModel.TypeConverter.lsgke4dxdx.wasm",
        "hash": "sha256-BQDbUp5Yfsh/HxRpj7oWWTHoJSkB/pR0L4cBQgB90R4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.wasm",
        "name": "System.ComponentModel.a5ymz1dgkp.wasm",
        "hash": "sha256-ojK0ApYRrqL20OXnAZjlCAZq3bjC9PdHBcCx1Yg4yCw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Console.wasm",
        "name": "System.Console.mp2jhdra6h.wasm",
        "hash": "sha256-esnKDUvuj0tviyfDXDtfLSKIE9hA8ZgsuSSJvOMB+nI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Data.Common.wasm",
        "name": "System.Data.Common.rjaxg8kaei.wasm",
        "hash": "sha256-WVwXE6+YXEyBCy3GvF/q6yugU3dpjsC8oYfB2xZYCUo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.DiagnosticSource.wasm",
        "name": "System.Diagnostics.DiagnosticSource.yqzo959hoj.wasm",
        "hash": "sha256-+ncbIz1lYkNEVzpCfwKYV3zSvShXfv80+975q/LF8EQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.Process.wasm",
        "name": "System.Diagnostics.Process.a8ziyocvze.wasm",
        "hash": "sha256-2sRdRbKIORcep4q7G6TTUuDibVQYEv560S85E+9dKEg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.TextWriterTraceListener.wasm",
        "name": "System.Diagnostics.TextWriterTraceListener.jsbg7r8t7f.wasm",
        "hash": "sha256-sxIHsWkxlUYGxr5Y1vKhr4QxUEm8N79csn/9/XmGc/g=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.TraceSource.wasm",
        "name": "System.Diagnostics.TraceSource.kc9k1de6ls.wasm",
        "hash": "sha256-VMHv5K7YwctxcQWj2nTIPjRVGO4h+Z7zNQbQ9w9+r88=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Drawing.wasm",
        "name": "System.Drawing.c6tmp1gjc8.wasm",
        "hash": "sha256-NFpXLgCTsyBU+AK+27QTsQQxyAoaLo/HDVh9RRvDIL4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Drawing.Common.wasm",
        "name": "System.Drawing.Common.ayojf3fjrc.wasm",
        "hash": "sha256-7u/lqsff6AHFGtk5/19qpoIhFxtOz8drntgWLWkVuYs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Drawing.Primitives.wasm",
        "name": "System.Drawing.Primitives.x1fo8wr37x.wasm",
        "hash": "sha256-NVpP5JDBOWZ0I4T8pYhTnTg1Mrc+KF347FPAt0+5Hnk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Globalization.wasm",
        "name": "System.Globalization.lf410knpr2.wasm",
        "hash": "sha256-sUxXJh2fpF5hzzGgfu7db2owRzJYOdxAB7w+pZ19qZY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Compression.wasm",
        "name": "System.IO.Compression.7pl815q6l2.wasm",
        "hash": "sha256-JesQBKJqbr7ZwEDGtanb1PdldATAzEfTaTLuj47huAo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.MemoryMappedFiles.wasm",
        "name": "System.IO.MemoryMappedFiles.9ojxkhkatq.wasm",
        "hash": "sha256-2RnukMOg491e1Da7YzXcrnJq8eHmNumCAN9dDjPM9UM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Packaging.wasm",
        "name": "System.IO.Packaging.xu878h3o7s.wasm",
        "hash": "sha256-mURRqTAOXyUiC5Y5ZGVmMb+cFFrWxtkOLPr4irRsrPg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Pipelines.wasm",
        "name": "System.IO.Pipelines.px0vglstfo.wasm",
        "hash": "sha256-fZpB2NFmoDJXAqnCYU4+XAKkZbE51ViM3KsQ+bxqVOo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Linq.Expressions.wasm",
        "name": "System.Linq.Expressions.xw2hlj2biy.wasm",
        "hash": "sha256-/5UqRALZa8fMXdnBERbBPaQ56OlnYLihi7OIXOif75g=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Linq.wasm",
        "name": "System.Linq.k4wqhn36fh.wasm",
        "hash": "sha256-3L0+b9WaIInz82xN1xQnNvVc1YwS1XpUXy0HkM124xQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Memory.wasm",
        "name": "System.Memory.2d2824qlqv.wasm",
        "hash": "sha256-9m+M0U7XoZFZ5YUGy+/WyWQ7KlVJ8csNw5ZZ/2hsqaw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Http.wasm",
        "name": "System.Net.Http.rhytkc91vf.wasm",
        "hash": "sha256-32dGsTBf0XxbOmKvyOJr5Caj4ClDaYJCDRbdojpt1uQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Primitives.wasm",
        "name": "System.Net.Primitives.llz1zzlk2t.wasm",
        "hash": "sha256-eSkmLWnZ3RkSwQTH7dunFXMwU+BM4C35r9dvdzASvUk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Requests.wasm",
        "name": "System.Net.Requests.3xo1nchctk.wasm",
        "hash": "sha256-mZTZFmNpl/LQ86XO8I5aHp/MF9tC9s2RvG5OmA3S6yU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ObjectModel.wasm",
        "name": "System.ObjectModel.enlsj2x5xn.wasm",
        "hash": "sha256-muc1s+cgCXYY3qv90KvJkzK+7FX8U47tTTaZQdavuHk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Private.Uri.wasm",
        "name": "System.Private.Uri.af3rrtxbcp.wasm",
        "hash": "sha256-FqkpMJXVayFfWMUeRb19VDnj9XqLxqvceOXDFeotCbM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Private.Xml.wasm",
        "name": "System.Private.Xml.x22d2row4s.wasm",
        "hash": "sha256-N6b+YeQ6enutEhgH5BTFeWbgWbhF1q6++yqtdpaHWuE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Private.Xml.Linq.wasm",
        "name": "System.Private.Xml.Linq.n3gjul18m8.wasm",
        "hash": "sha256-QzplEt3BPWMGM059AY3zKPpIWhUt2mZF3F1QTXqaiQw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reactive.wasm",
        "name": "System.Reactive.5b9tzd9z66.wasm",
        "hash": "sha256-n4T369qSaHjeEnb3Iqc4E6kD91guKZUzFWiZI5D4aTI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.Metadata.wasm",
        "name": "System.Reflection.Metadata.j8tej8qlyx.wasm",
        "hash": "sha256-fYFwdBrxIwCDLJ8Yyx8hPrCa1dtDAJDSDl5e7aCqSoU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Numerics.wasm",
        "name": "System.Runtime.Numerics.fm07xoq2yg.wasm",
        "hash": "sha256-5z/I7Cg/sSoN/+WK0uvPYNvwCe8apdRbyK/4i0vylto=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Serialization.Formatters.wasm",
        "name": "System.Runtime.Serialization.Formatters.9pbq9tghlg.wasm",
        "hash": "sha256-vTqm1PUsIgZjHRnw4xJ43v/qSwmmqAGqhcxuXewckNc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Serialization.Primitives.wasm",
        "name": "System.Runtime.Serialization.Primitives.64aavri0zb.wasm",
        "hash": "sha256-SsPwD1pTQf/K+8zRrkqv8ooD2eP6PPCdWpYL39R1Jwg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.wasm",
        "name": "System.Security.Cryptography.tmaleqogwd.wasm",
        "hash": "sha256-FR8M8I3fqa5faZ9UFZ66slX+7g6OIYh+8FOMevT8+8M=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.Encoding.CodePages.wasm",
        "name": "System.Text.Encoding.CodePages.s8q469tp7w.wasm",
        "hash": "sha256-3PecJ0mQoUsvNESid4xEcDynYb5zu3W2iqxpxHk100c=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.Encodings.Web.wasm",
        "name": "System.Text.Encodings.Web.gve7r38p7j.wasm",
        "hash": "sha256-DO44lAVZJmkVVZunAKMdhJBzZfTiSzbRUiqOjt2aHQA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.Json.wasm",
        "name": "System.Text.Json.ikx8e1xfmy.wasm",
        "hash": "sha256-DmIzcMJMoeKq7ouFDPmhHqfIP62XRISO1BKHvco6xik=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.RegularExpressions.wasm",
        "name": "System.Text.RegularExpressions.4f162xe0fl.wasm",
        "hash": "sha256-sJLMWVCnwC9b8GQpacOZTZf3t2hhsUL8kQwQoJ2I4hY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Tasks.Parallel.wasm",
        "name": "System.Threading.Tasks.Parallel.w6jfaw4jgj.wasm",
        "hash": "sha256-6NhGT3d4eoIAbRgQVnqzziwq547ETaBBiW6fAeKUYg0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Web.HttpUtility.wasm",
        "name": "System.Web.HttpUtility.0g89ximt2u.wasm",
        "hash": "sha256-AHwGsWordcYbsAtX116SV+OorkDfhXNO03zER6dFt7Q=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.Linq.wasm",
        "name": "System.Xml.Linq.pzw688hdq1.wasm",
        "hash": "sha256-e5txtle0V/jlZ+5laxHe+k2o2xewHh66AVpDrj8yXlc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.wasm",
        "name": "System.400l3mulw9.wasm",
        "hash": "sha256-vPEm2KqgXqruGwnz718iSgmDoB8Img7iH3FzO57b0Rg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "TextMateSharp.Grammars.wasm",
        "name": "TextMateSharp.Grammars.otf09rpeig.wasm",
        "hash": "sha256-5KrH0Jk8v0WmJYqsMpeRkVs2Dpr0lwfJDA3FXmnm02U=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "TextMateSharp.wasm",
        "name": "TextMateSharp.6md4kd9c6v.wasm",
        "hash": "sha256-Emo9Ed63D+n9oZs8t89jSOqDdCfySWhUuIknZxel//8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "ThemeEditor.Controls.ColorPicker.wasm",
        "name": "ThemeEditor.Controls.ColorPicker.fclerks6wc.wasm",
        "hash": "sha256-j27egpQcEOhUj8HdWsRLHH72iJOMzDM8TdXd2kfQ6iY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "UglyToad.PdfPig.Core.wasm",
        "name": "UglyToad.PdfPig.Core.onstv4oevl.wasm",
        "hash": "sha256-VEWeRtzu9hpQbsoL8OloQlEkwz87682XrLZYz5yaPDo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "UglyToad.PdfPig.Fonts.wasm",
        "name": "UglyToad.PdfPig.Fonts.287wmgrafz.wasm",
        "hash": "sha256-kjJVOWsHULAI4bvXmya1gpx1ulIL1MyKum0Ako1wjHg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "UglyToad.PdfPig.Rendering.Skia.wasm",
        "name": "UglyToad.PdfPig.Rendering.Skia.1m6b02vrtd.wasm",
        "hash": "sha256-Oe5RnsPAEkB1/05LrlGXUcHcRWHmC5TCLU5u3qPq2Ek=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "UglyToad.PdfPig.Tokenization.wasm",
        "name": "UglyToad.PdfPig.Tokenization.g7831dtges.wasm",
        "hash": "sha256-tO5Cpn4JM1n+EQlOhnbVCdu5HJirPC5rWajiktcm9Pg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "UglyToad.PdfPig.Tokens.wasm",
        "name": "UglyToad.PdfPig.Tokens.r0ektomzmm.wasm",
        "hash": "sha256-+0ABJtBE76RNNN+B2cF8JStfXLAdpk1JUGBLQ5ZITow=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "UglyToad.PdfPig.wasm",
        "name": "UglyToad.PdfPig.dz8evb8q3b.wasm",
        "hash": "sha256-B1CcoRZozh7ybEhakl8vT27OowHTAexBVN09O+O48ak=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Vello.Native.wasm",
        "name": "Vello.Native.ngqll281cq.wasm",
        "hash": "sha256-yUmWmTtEDImav2AGW1Z7UFFZfL8GLtF3y7RfoAB2epI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Vello.wasm",
        "name": "Vello.5la0upv64k.wasm",
        "hash": "sha256-Z3goMCmViJTAbjswE6tR067ksn9/qThH4miNRpN+1kE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "VelloSharp.Core.wasm",
        "name": "VelloSharp.Core.s3dweg76mv.wasm",
        "hash": "sha256-qPcm6P9DI2nFbHqYuBC+X02QU/5M8XHJDDvu9si9NNI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "VelloSharp.Ffi.Core.wasm",
        "name": "VelloSharp.Ffi.Core.0dv4r6lk5d.wasm",
        "hash": "sha256-PGUmmKuPrf3F866aLAMZiKzeQFwi1tK3Igr2KlFmUto=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "VelloSharp.Ffi.Gpu.wasm",
        "name": "VelloSharp.Ffi.Gpu.zcfh0pdwhg.wasm",
        "hash": "sha256-/cM8Gq7KZsyXF9I9dECr6KRlJaR+awlfSQBOexasDhQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "VelloSharp.Ffi.Sparse.wasm",
        "name": "VelloSharp.Ffi.Sparse.9b1rwhkxp9.wasm",
        "hash": "sha256-mzyziO671+WZt0sf6ptOwGfMbqRicxrCAdNshNxGLNk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "VelloSharp.Rendering.wasm",
        "name": "VelloSharp.Rendering.scx4nnsg65.wasm",
        "hash": "sha256-JIsmN4S7k9viZ7MsVOdM0BSSHRWf8dSHupH/27/i1Ew=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "VelloSharp.wasm",
        "name": "VelloSharp.9663jfwg9s.wasm",
        "hash": "sha256-/y4YHe8ruyqueB+wWi/GR3kkMRcg+WWoQEIoZwdgngw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Xaml.Behaviors.Interactions.Custom.wasm",
        "name": "Xaml.Behaviors.Interactions.Custom.op4wxxbjqt.wasm",
        "hash": "sha256-3DT2fvIUzkc1HzZO+pkcaat/BND8ANlHxyXB2NLplE8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Xaml.Behaviors.Interactions.DragAndDrop.wasm",
        "name": "Xaml.Behaviors.Interactions.DragAndDrop.lfknxwyfv6.wasm",
        "hash": "sha256-zxfgpiWaFa+k4zbsn2Tlhogykaok3kN8DAmHzDxkriw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Xaml.Behaviors.Interactivity.wasm",
        "name": "Xaml.Behaviors.Interactivity.ysj9hgjsg3.wasm",
        "hash": "sha256-gu19yVroaH4Zm8nGOZDGVKfuIRdFkrgqFAP9NCbjGKk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "netDxf.netstandard.wasm",
        "name": "netDxf.netstandard.t085q9h5tz.wasm",
        "hash": "sha256-I9oxQRJWc8uDJlyQiNtXmfn3we94S965TMmb1+oEEGg=",
        "cache": "force-cache"
      }
    ],
    "satelliteResources": {
      "cs": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.7tr8g6km7o.wasm",
          "hash": "sha256-y+Y9ETvLTx9R+z7EBqgNmIBqQLEv0zhjgv0T7GuVaxw=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.ncyjg21q77.wasm",
          "hash": "sha256-pxzSGpQdM/slmFBgQf8QMehjkKWMozPJYzfPqW5HeVc=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.wnbdauwcjs.wasm",
          "hash": "sha256-TP4YHlq4+1kYzEcUFGYearhu1ghIjX5pqGVYi7ZnHzk=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.azbzwnm97j.wasm",
          "hash": "sha256-mlyPvIy/aTrD9cWR+naU3dIlgyi0Hd5XLyFjgSiNIwo=",
          "cache": "force-cache"
        }
      ],
      "de": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.s3jug86fll.wasm",
          "hash": "sha256-yPMxBBSE7GWJqcYGO41ONqEhkW5oSsVshAP1sjHMfhI=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.nrmkzk2qqa.wasm",
          "hash": "sha256-QyPHORDObecDXST5xMuf7MOjs86CBuGI3xViI+xovF8=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.d09sl02o64.wasm",
          "hash": "sha256-nXF46oooz9EfI7afL+/D881m5+Zb3YQoc6pp8bQN50I=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.uxm2gznvff.wasm",
          "hash": "sha256-XnOwMjY5/+DsrMdTu6tlf+FF5JbUGf0DZCeqDZrS9Nk=",
          "cache": "force-cache"
        }
      ],
      "es": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.mdxb0xc90d.wasm",
          "hash": "sha256-Rl6P31C3wd2SP+yqATVvHH08i9d5PnfrgMjEi5K3VA8=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.7u63wh3avx.wasm",
          "hash": "sha256-oQbGJX2FzuOnM40Kb8YEjIQax6I+o1FvPXyTpMfVepI=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.0enknokone.wasm",
          "hash": "sha256-aMoNM8looLAY7doMdJ+dkBPTl0Xc+8l5Wn10jwnBHMU=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.ec0xfu5e6x.wasm",
          "hash": "sha256-/vbzh2wSibkLG+hxBNdOvithq0WREjHZI9USfqETzd0=",
          "cache": "force-cache"
        }
      ],
      "fr": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.or7kvtcugc.wasm",
          "hash": "sha256-rEjyH3R4JbTDxCEuMDfyZMOHxyEWPBztw+ZPzVyDXxI=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.yiwbzamycv.wasm",
          "hash": "sha256-ljtkxxDpZ57IdAOGi0BjhomnAssAojYKklGmb+MxuBs=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.a0qu1k1uee.wasm",
          "hash": "sha256-1h+LaqewUT3kXYjbdKRDql88o8qlVbCk7GYvgq1fvgQ=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.udwqjg1mda.wasm",
          "hash": "sha256-jhZ+Q0gSqxX6phNcgIwZy9p0Pz7EdbFCrXBrtsls1mA=",
          "cache": "force-cache"
        }
      ],
      "it": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.77fsg4dpm3.wasm",
          "hash": "sha256-DRRtsRtKo/6QKinzq3gDV6NXJIt7zAB+KvBNhjl7CBI=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.465xyiyi1i.wasm",
          "hash": "sha256-lN03J4Ux8x+ewgl0TIBsNWX5yySMVEuOtITVTXKeh4w=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.cvwojecj65.wasm",
          "hash": "sha256-mNgU0hRwHqGvQXkjE8E3YbdT3NkN0WRTui5Wpop980g=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.86tqtx2upv.wasm",
          "hash": "sha256-0OjnaSwlweTjnDhi+nfZTko7nCMlOA+fGiL6fml5V10=",
          "cache": "force-cache"
        }
      ],
      "ja": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.kw2so9wmms.wasm",
          "hash": "sha256-TKZTMMdizcGbpSNvESI2M61YM4FnGECMF/QIBjsKCfg=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.hi9e1rrn8r.wasm",
          "hash": "sha256-qYT18CC0AWdwbfE90bB/q/PeXLn6pjvSj1HcHfGpm34=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.xlaf8g50im.wasm",
          "hash": "sha256-i4QIqxklOIr5Sad5FrqJxQ+MMLIMoBuCRLXbJhoMjGo=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.ner1z7do1e.wasm",
          "hash": "sha256-AXAAYzbCWQvP4VWolOBBZDvPJ/hxjI8p6bfsg6VqimQ=",
          "cache": "force-cache"
        }
      ],
      "ko": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.xfsvx90w35.wasm",
          "hash": "sha256-vSYzyW/yaagFvXeFvwNi7NINlCBX9xEgf6Zr9CTib28=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.998gjfe8qz.wasm",
          "hash": "sha256-MwPjNwHYVz3X3xvxzDupsG637ubEm65TVIPbS6fvL94=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.k6f9fxcwcy.wasm",
          "hash": "sha256-HAh3/4btNeglZ7Pmu3sNe6qQFrexl5ZWLGHLzH8MZn0=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.u9mscaqe3g.wasm",
          "hash": "sha256-wnNUgEa31P8Bvdti52QP1fOQQcvvViQDmMtMP2nnl18=",
          "cache": "force-cache"
        }
      ],
      "pl": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.o8is0tcat6.wasm",
          "hash": "sha256-qOA+puO+AeTNrqba3dj1DCNbrDVHR3LOXa85NG9S9ko=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.snlz6ty14s.wasm",
          "hash": "sha256-mO3VU//2bzWuSiCo2L/1+LmQ1T/1R9Cp/TjbZFTFz9U=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.85n1pcgl3t.wasm",
          "hash": "sha256-bNQ0rxEtUlVvVb6reRj8tdc1yal1iVU5wLiwp3rA/Ic=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.si9hlaf1s0.wasm",
          "hash": "sha256-dBft16QYqFgHThAnfDbPL14GgZO4xew3FuT2RvSJvDE=",
          "cache": "force-cache"
        }
      ],
      "pt-BR": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.h3tvt6yhes.wasm",
          "hash": "sha256-DRpOHE2IaeAF8t1Z3W4yWmFvYsRSmv6+Y+jS4pcSkuI=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.xarwohe7gy.wasm",
          "hash": "sha256-Oe6BxpPIYbJqQfzGcO8X/T/WeFUPxQ0mLYPPZnegLOc=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.btsh2n0vep.wasm",
          "hash": "sha256-H2qwHKIJpFQEvTGfoPtmv3BSHoR71PddJ8UPDAe9ivk=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.mn8z1j7j2b.wasm",
          "hash": "sha256-khOT3LEMbAh8DDtztJPeiiuLYDPdIBD6ckqK5W5lRG4=",
          "cache": "force-cache"
        }
      ],
      "ru": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.uyw3xgu151.wasm",
          "hash": "sha256-purVF0dWPK8BHRy/VJbM4QnyjUxS3+GgccBc52sxDtg=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.u5qr7p1vic.wasm",
          "hash": "sha256-zgdEQNh/n56dpU21YmaNRBf0ARPO5DALw3JGr5HMjQQ=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.3r9op2t2kp.wasm",
          "hash": "sha256-oWcPOsWLfSOw2HV38k6xo2J3K07BAcEi+JYMUSIg0yw=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.h3mfsuyol9.wasm",
          "hash": "sha256-Y+54eupiktzb2rnvJIfO8kTru7uv7j4SEncDmfjOe5Y=",
          "cache": "force-cache"
        }
      ],
      "tr": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.ztp2t39rbt.wasm",
          "hash": "sha256-uZcP6mXqmrujdxjUZErec6uYbbVkodLUjE9Cr95vgmI=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.pwlsqlbbsk.wasm",
          "hash": "sha256-F2QjCvdgMhil7A5fgAZhgxmj4ws4A6BqTryGCfkQ3fI=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.d420wkh7zs.wasm",
          "hash": "sha256-vcrvcb3ZntICOjkotvbO/SCpzcAUBY7AWEjg2YqAxL0=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.5zvghpcemm.wasm",
          "hash": "sha256-HwW8NLN7cTqlZfYe16gB/a3QCBO6ZGKi81rApQRTec4=",
          "cache": "force-cache"
        }
      ],
      "zh-Hans": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.3uvaw3i9q1.wasm",
          "hash": "sha256-qytJtojBy1xQa9bi7XI/vpHBWJBHD51Z2hw9TTLCtog=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.xa5czk39rr.wasm",
          "hash": "sha256-Jxfdz3Jgmon5wrrZgjDG6+hTArh5VrZeQblT1tZuePA=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.h75hdwb043.wasm",
          "hash": "sha256-Y27sByZLGY3AgSRQvNZYjXXi98zHqULG+bKHf7aByxw=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.v3jjn2jlhr.wasm",
          "hash": "sha256-helIZuiJmOS4iSCte80+TeV/keycWw4zMTad7TECc50=",
          "cache": "force-cache"
        }
      ],
      "zh-Hant": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.Scripting.resources.pn0k8ji6e9.wasm",
          "hash": "sha256-VRSMmawElm49PSIOpWFHNWCDg7AJ521r97zOs1H1vR8=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.wasm",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.pc7lskmtva.wasm",
          "hash": "sha256-eSTRMqYhBX0GTXPnSh4dY0hu7n/XPeRSmwIrNOcAOVE=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.wasm",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.48h41ik7t3.wasm",
          "hash": "sha256-zKZZKzwrqOahrfi6G3Z9gzpXzikE4NWfu4e1vx6ej/0=",
          "cache": "force-cache"
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.wasm",
          "name": "Microsoft.CodeAnalysis.resources.xff2phc93y.wasm",
          "hash": "sha256-rVcHpYZCLuYlbIqfBE0mN4yXFwZCS2YbR7T5PyoeNo0=",
          "cache": "force-cache"
        }
      ]
    }
  },
  "debugLevel": 0,
  "linkerEnabled": true,
  "globalizationMode": "sharded",
  "runtimeConfig": {
    "runtimeOptions": {
      "configProperties": {
        "MVVMTOOLKIT_ENABLE_INOTIFYPROPERTYCHANGING_SUPPORT": true,
        "Microsoft.Extensions.DependencyInjection.VerifyOpenGenericServiceTrimmability": true,
        "System.ComponentModel.DefaultValueAttribute.IsSupported": false,
        "System.ComponentModel.Design.IDesignerHost.IsSupported": false,
        "System.ComponentModel.TypeConverter.EnableUnsafeBinaryFormatterInDesigntimeLicenseContextSerialization": false,
        "System.ComponentModel.TypeDescriptor.IsComObjectDescriptorSupported": false,
        "System.Data.DataSet.XmlSerializationIsSupported": false,
        "System.Diagnostics.Debugger.IsSupported": false,
        "System.Diagnostics.Metrics.Meter.IsSupported": false,
        "System.Diagnostics.Tracing.EventSource.IsSupported": false,
        "System.Globalization.Invariant": false,
        "System.TimeZoneInfo.Invariant": false,
        "System.Linq.Enumerable.IsSizeOptimized": true,
        "System.Net.Http.EnableActivityPropagation": false,
        "System.Net.Http.WasmEnableStreamingResponse": true,
        "System.Net.SocketsHttpHandler.Http3Support": false,
        "System.Reflection.Metadata.MetadataUpdater.IsSupported": false,
        "System.Resources.ResourceManager.AllowCustomResourceTypes": false,
        "System.Resources.UseSystemResourceKeys": true,
        "System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported": true,
        "System.Runtime.InteropServices.BuiltInComInterop.IsSupported": false,
        "System.Runtime.InteropServices.EnableConsumingManagedCodeFromNativeHosting": false,
        "System.Runtime.InteropServices.EnableCppCLIHostActivation": false,
        "System.Runtime.InteropServices.Marshalling.EnableGeneratedComInterfaceComImportInterop": false,
        "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization": false,
        "System.StartupHookProvider.IsSupported": false,
        "System.Text.Encoding.EnableUnsafeUTF7Encoding": false,
        "System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault": false,
        "System.Threading.Thread.EnableAutoreleasePool": false
      }
    }
  }
}/*json-end*/);export{gt as default,ft as dotnet,mt as exit};
