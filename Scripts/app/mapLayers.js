/* Establish array to hold both Tweet layer and Trends layer */
var overlays = [];
/*-- Begin Tweet overlay --*/
var CustomOverlay;
CustomOverlay.prototype = new google.maps.OverlayView();

function CustomOverlay(map,hidden) {
    overlays.push(this);
    this.overlays = []; // holds either tweet or trend markers
    this.hidden = hidden;
    if(!this.hidden) {
        this.setMap(map);
    }
}

CustomOverlay.prototype.addOverlay = function (overlay) {
    
    if (this.getMap() !== null) {
        this.overlays.push(overlay);
        this.updateOverlays();
    }
};

CustomOverlay.prototype.getState = function () {
    return this.hidden;
};
CustomOverlay.prototype.updateOverlays = function () {
    if(this.getMap() != null) {
        for (var i = 0; i < this.overlays.length; i++) {
            if (this.overlays[i].getMap() == null) {
                //console.log('yes this is being triggered');
               this.overlays[i].setMap(this.getMap());
            }
        }
    }
};
CustomOverlay.prototype.hide = function () {
    this.setMap(null);
    this.hidden = true;
    for (var i = 0; i < this.overlays.length; i++) {
        // this.overlays[i].setMap(null);
        this.overlays[i].setVisible(false);
    }
};
CustomOverlay.prototype.show = function () {
    this.setMap(map);
    this.hidden = false;
    for (var i = 0; i < this.overlays.length; i++) {
        // this.overlays[i].setMap(map);
        this.overlays[i].setVisible(true); // use setVisible as setMap is too verbose and decouples from DOM
    }
};
CustomOverlay.prototype.draw = function () { };
CustomOverlay.prototype.onAdd = CustomOverlay.prototype.updateOverlays;
CustomOverlay.prototype.onRemove = CustomOverlay.prototype.updateOverlays;
/*-- End Tweet overlay --*/

// /*-- Begin Trends overlay --*/
// var TrendsOverlay;
// TrendsOverlay.prototype = new google.maps.OverlayView();

// function TrendsOverlay(map,hidden) {
//     this.overlays = [];
//     this.hidden = hidden;
//     this.setMap(map);
// }
// TrendsOverlay.prototype.getState = function () {
//     return this.hidden;
// };
// TrendsOverlay.prototype.addOverlay = function (overlay) {
//     if (!this.hidden) {
//         this.overlays.push(overlay);
//         this.updateOverlays();
//         //console.log('es');
//     }
// };
// TrendsOverlay.prototype.updateOverlays = function () {
//         for (var i = 0; i < this.overlays.length; i++) {
//             if (this.overlays[i].getMap() == null) {
//                 //console.log('yes this is being triggered');
//                 this.overlays[i].setMap(this.getMap());
//             }
//         }
// };
// TrendsOverlay.prototype.hide = function () {
//     this.setMap(null);
//     this.hidden = true;
//     for (var i = 0; i < this.overlays.length; i++) {
//         this.overlays[i].setMap(null);
//     }
// };
// TrendsOverlay.prototype.show = function () {
//     this.setMap(map);
//     this.hidden = false;
//     for (var i = 0; i < this.overlays.length; i++) {
//         this.overlays[i].setMap(this.getMap());
//     }
// };
// TrendsOverlay.prototype.draw = function () { };
// TrendsOverlay.prototype.onAdd = TrendsOverlay.prototype.updateOverlays;
// TrendsOverlay.prototype.onRemove = TrendsOverlay.prototype.updateOverlays;
// /*-- End Trends overlay --