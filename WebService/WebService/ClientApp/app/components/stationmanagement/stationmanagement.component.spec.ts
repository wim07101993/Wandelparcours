import { inject, TestBed} from '@angular/core/testing';

import {StationmanagementComponent} from './stationmanagement.component';
import {RestServiceService} from "../../service/rest-service.service";
import {HttpModule} from "@angular/http";

import {RenderBuffer,} from "../../helpers/RenderBuffer"
import {Station} from "../../models/station";


describe('StationmanagementComponent', () => {
    let stationManagemet: StationmanagementComponent;

    beforeEach(() => {
        let serviceStub = {
            LoadStations: async (station: StationmanagementComponent) => {
                station.stationMacAdresses.push("azerty");
                return true;
            },
            SaveStationToDatabase: async (station: Station) => {
                stationManagemet.stations.set(station.mac, station.position);
                return true;
            },
            DeleteStation: async (mac: string) => {
                stationManagemet.stations.clear();
                return true;
            }

        };

        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [{provide: RestServiceService, useValue: serviceStub}]
        });

    });
    beforeEach(inject([RestServiceService], (service: RestServiceService) => {
        stationManagemet = new StationmanagementComponent(service);
        stationManagemet.renderBuffer = new RenderBuffer(stationManagemet);
    }));
    describe("Station management rest service", () => {

        it('should create', () => {
            expect(stationManagemet).toBeTruthy();
        });
        it('Load stations', inject([RestServiceService], (service: RestServiceService) => {
            service.LoadStations(stationManagemet).then(e => {
                if (<boolean>e == true && stationManagemet.stationMacAdresses.length != 0) {
                    expect(true).toBeTruthy();
                } else {
                    expect(false).toBeTruthy();
                }
            });
        }));
        it('Save stations', inject([RestServiceService], (service: RestServiceService) => {
            let station = new Station();
            station.mac = "testing";
            service.SaveStationToDatabase(station).then(e => {
                if (<boolean>e == true) {
                    try {
                        let test = stationManagemet.stations.get(station.mac);
                        if (test != undefined)
                            expect(true).toBeTruthy();
                        else
                            expect(false).toBeTruthy();
                    } catch (e) {
                        expect(false).toBeTruthy();
                    }

                } else {
                    expect(false).toBeTruthy();
                }
            });
        }));

        it('delete stations', inject([RestServiceService], (service: RestServiceService) => {
            let station = new Station();
            station.mac = "testing";
            service.SaveStationToDatabase(station).then(e => {
                service.DeleteStation(station.mac).then(() => {
                    try {
                        if (stationManagemet.stations.get(station.mac) != null) {
                            expect(false).toBeTruthy();
                        } else {
                            expect(false).toBeFalsy();
                        }
                    } catch (e) {
                        expect(false).toBeFalsy();
                    }
                });

            });
        }));
    });

});
