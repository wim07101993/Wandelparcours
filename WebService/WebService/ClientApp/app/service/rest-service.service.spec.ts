import {inject, TestBed} from '@angular/core/testing';

import {RestServiceService} from './rest-service.service';
import {StationmanagementComponent} from "../components/stationmanagement/stationmanagement.component";
import {HttpModule} from '@angular/http';


import {RenderBuffer,} from "../helpers/RenderBuffer"
import {Station} from "../models/station";

describe('RestServiceService', () => {

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [RestServiceService]
        });

    });


    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [RestServiceService]
        });
    });

    it('should be created', inject([RestServiceService], (service: RestServiceService) => {
        expect(service).toBeTruthy();
    }));
    
    describe("StationManagement test", () => {
        let stationManagemet:StationmanagementComponent;
        beforeEach(inject([RestServiceService], (service: RestServiceService) => {
            stationManagemet=new StationmanagementComponent(service);
            stationManagemet.renderBuffer=new RenderBuffer(stationManagemet);
            
        }));
        
        it('Test load stations error ', inject([RestServiceService], (service: RestServiceService) => {
            service.restUrl = "http://qsdf/";
            let runExpect = (value: boolean) => {
                expect(value).toBeFalsy();
            };
            service.LoadStations(stationManagemet).then((e) => {
                let test=<boolean>e; 
                runExpect(test);
            }).catch(() => {
                runExpect(false);

            });
        }));
        it('Test load stations succesfull ', inject([RestServiceService], (service: RestServiceService) => {
            let runExpect = (value: boolean) => {
                expect(value).toBeTruthy();
            };
            service.LoadStations(stationManagemet).then((e) => {
                console.log(e);
                runExpect(true);
            }).catch(() => {
                runExpect(false);

            });
        }));


        it('Test delete stations  error ', inject([RestServiceService], (service: RestServiceService) => {
            let runExpect = (value: boolean) => {
                expect(value).toBeFalsy();
            };
            service.DeleteStation("teststation").then((e) => {
                let test=<boolean>e;
                runExpect(test);
            }).catch(() => {
                runExpect(false);

            });
        }));

        it('Test insert stations  succes ', inject([RestServiceService], (service: RestServiceService) => {
            let runExpect = (value: boolean) => {
                expect(value).toBeTruthy();
            };
            let station =new Station();
            
            station.mac = "teststation";
            service.SaveStationToDatabase(station).then((e) => {
                let test=<boolean>e;
                runExpect(test);
            }).catch(() => {
                runExpect(false);

            });
        }));

        it('Test insert stations  fail ', inject([RestServiceService], (service: RestServiceService) => {
            let runExpect = (value: boolean) => {
                expect(value).toBeFalsy();
            };
            service.restUrl="http://qsdf/";
            let station =new Station();
            station.mac = "teststation";
            service.SaveStationToDatabase(station).then((e) => {
                let test=<boolean>e;
                runExpect(test);
            }).catch(() => {
                console.log("correct false");
                runExpect(false);

            });
        }));
        
        it('Test delete stations  success', inject([RestServiceService], (service: RestServiceService) => {
            let runExpect = (value: boolean) => {
                
                expect(value).toBeTruthy();
            };
            service.DeleteStation("teststation").then((e) => {
                let test=<boolean>e;
                runExpect(test);
            }).catch(() => {
                expect(false).toBeTruthy();

            });
        }));
    });

});
