export function getTrilateration(position1, position2, position3) {
    try {

        var center =(position1.x+position2.x,position3.x)/3;
        /*var dist = (()=>{
            var distances=[];
            distances.push(Math.abs(position1.x-position2.x));
            distances.push(Math.abs(position1.x-position3.x));
            distances.push(Math.abs(position2.x-position3.x));
            return Math.max([...distances]);
        })();

        var distCoef=Math.abs(position1.x-position2.x);*/

        var distCoef=1;
        var xCoef=24.165;
        var yCoef=22.65;
        var averageDist=(position1.distance+position2.distance+position3.distance)/3;
        var totdist=(position1.distance+position2.distance+position3.distance);
        var xa = position1.x*xCoef;
        var ya = position1.y*yCoef;
        var xb = position2.x*xCoef;
        var yb = position2.y*yCoef;
        var xc = position3.x*xCoef;
        var yc = position3.y*yCoef;
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
            x: x/xCoef,
            y: y/yCoef
        };
    } catch (error) {
        console.log(error);
        return null;
    }

}