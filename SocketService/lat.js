
var pos1={"x":0.36041666666666666,"y":0.8682308242666469,"distance":2.7478244367112157};
var pos2={"x":0.49015317286652077,"y":0.6627042924478191,"distance":3.1179030304806012};
var pos3={"x":0.6069444444444444,"y":0.8286416829085765,"distance":5.864496312255465};


// @param {getal:number}
var getal = 4;


getTrilateration(pos1,pos2,pos3);
function getTrilateration(position1, position2, position3) {
    try {

        var center =(position1.x+position2.x,position3.x)/3;
        var distCoef=Math.abs(position1.x-position2.x)/5;
        var averageDist=(position1.distance+position2.distance+position3.distance)/3;
        var totdist=(position1.distance+position2.distance+position3.distance);
        var xa = position1.x;
        var ya = position1.y;
        var xb = position2.x;
        var yb = position2.y;
        var xc = position3.x;
        var yc = position3.y;
        // var ra = position1.distance/averageDist*center;
        // var rb = position2.distance/averageDist*center;
        // var rc = position3.distance/averageDist*center;
        var ra = position1.distance*distCoef;
        var rb = position2.distance*distCoef;
        var rc = position3.distance*distCoef;
        var S = (Math.pow(xc, 2.) - Math.pow(xb, 2.) + Math.pow(yc, 2.) - Math.pow(yb, 2.) + Math.pow(rb, 2.) - Math.pow(rc, 2.)) / 2.0;
        var T = (Math.pow(xa, 2.) - Math.pow(xb, 2.) + Math.pow(ya, 2.) - Math.pow(yb, 2.) + Math.pow(rb, 2.) - Math.pow(ra, 2.)) / 2.0;
        var y = ((T * (xb - xc)) - (S * (xb - xa))) / (((ya - yb) * (xb - xc)) - ((yc - yb) * (xb - xa)));
        var x = ((y * (ya - yb)) - T) / (xb - xa);
        

        return {
            x: x,
            y: y
        };
    } catch (error) {
        console.log(error);
        return null;
    }

}