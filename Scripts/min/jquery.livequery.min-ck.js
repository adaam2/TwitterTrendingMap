!function(e){"function"==typeof define&&define.amd?define(["jquery"],e):"object"==typeof exports?module.exports=e:e(jQuery)}(function(e){e.extend(e.fn,{livequery:function(t,i,n){var r=e.livequery.findorcreate(this,t,i,n);return r.run(),this},expire:function(t,i,n){var r=e.livequery.find(this,t,i,n);return r&&r.stop(),this}}),e.livequery=function(t,i,n,r){this.selector=i,this.jq=t,this.context=t.context,this.matchedFn=n,this.unmatchedFn=r,this.stopped=!1,this.id=e.livequery.queries.push(this)-1,n.$lqguid=n.$lqguid||e.livequery.guid++,r&&(r.$lqguid=r.$lqguid||e.livequery.guid++)},e.livequery.prototype={run:function(){this.stopped=!1,this.jq.find(this.selector).each(e.proxy(function(e,t){this.added(t)},this))},stop:function(){this.jq.find(this.selector).each(e.proxy(function(e,t){this.removed(t)},this)),this.stopped=!0},matches:function(t){return!this.isStopped()&&e(t,this.context).is(this.selector)&&this.jq.has(t).length},added:function(e){this.isStopped()||this.isMatched(e)||(this.markAsMatched(e),this.matchedFn.call(e,e))},removed:function(e){!this.isStopped()&&this.isMatched(e)&&(this.removeMatchedMark(e),this.unmatchedFn&&this.unmatchedFn.call(e,e))},getLQArray:function(t){var i=e.data(t,e.livequery.key)||[],n=e.inArray(this.id,i);return i.index=n,i},markAsMatched:function(t){var i=this.getLQArray(t);-1===i.index&&(i.push(this.id),e.data(t,e.livequery.key,i))},removeMatchedMark:function(t){var i=this.getLQArray(t);i.index>-1&&(i.splice(i.index,1),e.data(t,e.livequery.key,i))},isMatched:function(e){var t=this.getLQArray(e);return-1!==t.index},isStopped:function(){return this.stopped===!0}},e.extend(e.livequery,{version:"2.0.0-pre",guid:0,queries:[],watchAttributes:!0,attributeFilter:["class","className"],setup:!1,timeout:null,method:"none",prepared:!1,key:"livequery",htcPath:!1,prepare:{mutationobserver:function(){var t=new MutationObserver(e.livequery.handle.mutationobserver);t.observe(document,{childList:!0,attributes:e.livequery.watchAttributes,subtree:!0,attributeFilter:e.livequery.attributeFilter}),e.livequery.prepared=!0},mutationevent:function(){document.addEventListener("DOMNodeInserted",e.livequery.handle.mutationevent,!1),document.addEventListener("DOMNodeRemoved",e.livequery.handle.mutationevent,!1),e.livequery.watchAttributes&&document.addEventListener("DOMAttrModified",e.livequery.handle.mutationevent,!1),e.livequery.prepared=!0},iebehaviors:function(){e.livequery.htcPath&&(e("head").append("<style>body *{behavior:url("+e.livequery.htcPath+")}</style>"),e.livequery.prepared=!0)}},handle:{added:function(t){e.each(e.livequery.queries,function(e,i){i.matches(t)&&setTimeout(function(){i.added(t)},1)})},removed:function(t){e.each(e.livequery.queries,function(e,i){i.isMatched(t)&&setTimeout(function(){i.removed(t)},1)})},modified:function(t){e.each(e.livequery.queries,function(e,i){i.isMatched(t)?i.matches(t)||i.removed(t):i.matches(t)&&i.added(t)})},mutationevent:function(t){var i={DOMNodeInserted:"added",DOMNodeRemoved:"removed",DOMAttrModified:"modified"},n=i[t.type];"modified"===n?e.livequery.attributeFilter.indexOf(t.attrName)>-1&&e.livequery.handle.modified(t.target):e.livequery.handle[n](t.target)},mutationobserver:function(t){e.each(t,function(t,i){"attributes"===i.type?e.livequery.handle.modified(i.target):e.each(["added","removed"],function(t,n){e.each(i[n+"Nodes"],function(t,i){e.livequery.handle[n](i)})})})}},find:function(t,i,n,r){var u;return e.each(e.livequery.queries,function(e,d){return i!==d.selector||t!==d.jq||n&&n.$lqguid!==d.matchedFn.$lqguid||r&&r.$lqguid!==d.unmatchedFn.$lqguid?void 0:(u=d)&&!1}),u},findorcreate:function(t,i,n,r){return e.livequery.find(t,i,n,r)||new e.livequery(t,i,n,r)}}),e(function(){if("MutationObserver"in window?e.livequery.method="mutationobserver":"MutationEvent"in window?e.livequery.method="mutationevent":"behavior"in document.documentElement.currentStyle&&(e.livequery.method="iebehaviors"),!e.livequery.method)throw new Error("Could not find a means to monitor the DOM");e.livequery.prepare[e.livequery.method]()})});