import {Injectable} from '@angular/core';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import {Resident} from '../models/resident';
import {Station} from '../models/station';
import {StationmanagementComponent} from '../components/stationmanagement/stationmanagement.component';
import { LoginService } from "./login-service.service";
@Injectable()
export class RestServiceService {
  [x: string]: any;

  url = 'http://localhost:5000/';

  get restUrl(): string {
    if (this.url === '') {
      return document.getElementsByTagName('base')[0].href;
    } else {
      return this.url;
    }
  }
  set restUrl(val: string) {
    this.url = val;
  }

  constructor(private login:LoginService) {}

  /**
   * Get all residents from database
   * @returns {Resident} residents of type Resident or undefined
   */
  async getAllResidents() {
      try {
          return  (await this.login.axios.get("/api/v1/residents")).data;
          }catch (e) {
          this.customErrorHandler.updateMessage(e.toString());
      }
  }
  /**
   * get one resident and only the needed properties
   * @param uniqueIdentifier unique id of resident
   * @returns {Resident} one resident of type Resident with only the requested properties or undefined
   */
  async getResidentBasedOnId(uniqueIdentifier: string){
    try {
        const query = '?properties=firstName&properties=lastName&properties=room&properties=birthday&properties=doctor';
        return (await this.login.axios.get('/api/v1/residents/' + uniqueIdentifier + query)).data;
    }catch (e) {
        console.log(e.toString());
    }
  }
  /**
   * delete resident from database based on id
   * @param uniqueIdentifier unique id of resident
   * @returns message "Deleted" on succes or "Something went wrong" on error
   */
  async deleteResidentByUniqueId(uniqueIdentifier: string) {
    try{
      await this.login.axios.delete('/api/v1/residents/' + uniqueIdentifier)
    }catch (e) {
        console.log(e.toString());
    }
  }
  /**
   * Edit resident in database based on Resident object with properties and only the properties that have been changed
   * @param dataToUpdate Object Resident with all properties
   * @param changedProperties properties != dataToUpdate
   * @returns message "updated" on succes or "Could not update data!" on error.
   */
  async editResidentWithData(dataToUpdate: any, changedProperties: any) {
    let url: string = '?properties=' + changedProperties[0];
    for (let _i = 1; _i < changedProperties.length; _i++) {
      url += '&properties=' + changedProperties[_i];
    }
    try {
       return (await this.login.axios.put("/api/v1/residents" + url, dataToUpdate));
    }catch (e) {
        console.log("Error Message: "+ e.toString())
    }
  }
  /**
   * Add resident to database
   * @param data Object resident with all saved properties
   * @returns True or false based on succes and Console log Message "Saved resident to database" on succes or "Could not save resident to database" on error.
   */
  async addResident(dataOfResident: any) {
    try{
       return (await this.login.axios.post('/api/v1/residents', dataOfResident)).data
    }catch (e) {
        console.log(e.toString());
    }
  }
  /**
     * Profilepicture will always be null when a resident is created
     * When a picture is selected null -> picture (param)
     * @param uniqueIdentifier
     * @param picture
     * @returns {Promise<void>}
     */
  async addProfilePic(uniqueIdentifier: any, picture: any) {
    await this.login.axios.put('/api/v1/residents/' + uniqueIdentifier + '/picture', picture)
  }

  ////////
  //TAGS//
  ////////

  /**
     * Add a beaconTag to a resident
     * @param {string} uniqueIdentifier
     * @param beaconMinorNumber
     * @returns {Promise<any>}
     */
  async addTagToResident(uniqueIdentifier: string, beaconMinorNumber: any){
    try {
        return (await this.login.axios.post('/api/v1/residents/' + uniqueIdentifier + '/tags', beaconMinorNumber, {
          headers: {
            'Content-type' : 'application/json'
          }
        })).data
    }catch (e) {
        console.log('Errormessage: ' + e.toString())
    }
  }

  /**
     * Delete a beaconTag from a resident
     * @param {string} uniqueIdentifier
     * @param {string} uniqueTagId
     * @returns {Promise<any>}
     */
  async deleteTagFromResident(uniqueIdentifier: string, uniqueTagId: string) {
    try {
        await this.login.axios.delete('/api/v1/residents/' + uniqueIdentifier + '/' + uniqueTagId);
    }catch (e) {
        console.log('Errormessage : ' + e.toString());
    }
  }

  /////////
  //MEDIA//
  /////////

  /**
   * Add correct media to database
   * @param uniqueIdentifier string: unique resident ID
   * @param media formdata: media data
   * @param addMedia string: medialink
   * Returns true or false or updates error message
   */
  async addCorrectMediaToDatabase(uniqueIdentifier: any, media: any, addMedia: string) {
    try {
        return (await this.login.axios.post('/api/v1/residents/' + uniqueIdentifier + addMedia, media)).data;
    }catch (e) {
        console.log('Errormessage: ' + e.toString());
    }
  }

  /**
   * Get correct media of resident
   * @param uniqueIdentifier string: resident id
   * @param type string: urlLink of media
   * returns true or false--> updates errormessage
   */
  async getCorrectMediaOfResidentBasedOnId(uniqueIdentifier: string, type: string) {
    try {
         const response  = (await this.login.axios.get('/api/v1/residents/' + uniqueIdentifier + type)).data;
         return response;
    } catch (e) {
        console.log('Errormessage: ' + e.toString());
    }
  }

  /**
   * delete resident media based on uniqueid
   * @param uniqueIdentifier string: Resident id
   * @param uniqueMediaIdentifier string: Media id
   * @param type string: urlLink
   * returns true or false --> updates errormessage
   */
  async deleteResidentMediaByUniqueId(uniqueIdentifier: string, uniqueMediaIdentifier: string, type: string) {
    try {
      await this.login.axios.delete('/api/v1/residents/' + uniqueIdentifier + type + '/' + uniqueMediaIdentifier);
    }catch (e) {
        console.log('Errormessage: ' + e.toString())
    }
  }

  ////////////////
  //LOCALISATION//
  ////////////////

  async SaveStationToDatabase(station: Station) {
      try {
          await this.login.axios.post('/api/v1/receivermodules',station);
      }catch (e) {
          console.log('Errormessage: ' + e.toString());
      }
  }

  async DeleteStation(mac: string) {
      try {
          this.login.axios.delete('/api/v1/receivermodules/bymac/' + mac)
      }catch (e) {
          console.log('Errormessage: ' + e.toString())
      }
  }

  async UpdateStation(id: string, newMac: string) {
      try {
          const resp = await this.login.axios.put('/api/v1/receivermodules/'+ id + '/Mac', "'" + newMac + "'", {
              headers: {
                  'Content-type' : 'application/json'
              }
          });
          console.log(resp.status);
          if(resp.status >= 200 && resp.status < 300){
              return true;
          }
          else{
              return false;
          }

      } catch (e) {
          console.log('Errormessage: ' + e.toString());
      }
  }

  async LoadStations(parent: StationmanagementComponent) {
      if (parent.stations != undefined) {
          parent.stations.clear();
      }
      if (parent.renderBuffer.buffer != undefined) {
          parent.renderBuffer.buffer.clear();
      }
      if (parent.stationMacAdresses != undefined) {
          parent.stationMacAdresses = [];
      }
      try {
          this.login.axios.get('/api/v1/receivermodules').then(function (response) {
              const tryParse = <Array<any>>(response.data);
              let station: any;
              if (tryParse != undefined) {
                  for (station of tryParse) {
                      if (station == undefined) {
                          continue;
                      }
                      parent.stationMacAdresses.push(station.mac);
                      parent.stations.set(station.mac, station.position);
                      parent.stationsIds.set(station.mac, station.id);
                  }
              }
              return true;
          })
      } catch (e) {
          console.log('Errormessage: ' + e.toString());
      }
      /*return new Promise<boolean>(resolve => {
        this.http.get(this.restUrl + 'api/v1/receivermodules').subscribe(response => {

            const tryParse = <Array<any>>(response.json());

            let station: any;
            if (tryParse != undefined) {
              for (station of tryParse) {
                if (station == undefined) {
                  continue;
                }
                parent.stationMacAdresses.push(station.mac);
                parent.stations.set(station.mac, station.position);
                parent.stationsIds.set(station.mac, station.id);
              }
            }
            resolve(true);
          },
          error => {
            console.log('can\'t load stations');
            console.log(error);
            resolve(false);
          });
      });*/
    }

    ///////////////////
    //Global tracking//
    ///////////////////

    async getAllResidentsWithAKnownLastLocation(){
      try {
          const resp = (await this.login.axios.get('/api/v1/location/residents/lastlocation')).data;
          return resp;
      }catch (e) {
          console.log('Errormessage: '+ e.toString());
      }
    }

    async getOneResidentWithAKnownLastLocation(uniqueIdentifier: string){
      try {
            const query= "propertiesToInclude=id &propertiesToInclude=firstName&propertiesToInclude=lastName&propertiesToInclude=lastRecordedPosition";
            const resp = (await this.login.axios.get(`/api/v1/residents/${uniqueIdentifier}?${query}`)).data
            return resp;
      }catch (e) {
            console.log('Errormessage: ' + e.toString());
      }
    }
    async createUser(userName,password,userType){
        try {
            let login =await this.login.axios.post("/api/v1/users",{"userName":userName,"password":password,"userType":userType});
            return true;
        } catch (error) {
            return false;
        }
    }
}
