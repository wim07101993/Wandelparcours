import {Component, OnInit} from '@angular/core';
import {Resident} from '../../../models/resident';
import {ActivatedRoute, Router} from '@angular/router';
import {RestServiceService} from '../../../service/rest-service.service';
import {MediaService} from '../../../service/media.service';
import {NgForm} from '@angular/forms';
import { LoginService } from '../../../service/login-service.service';
declare var $: any;
declare var Materialize: any;

@Component({
  selector: 'app-personalia',
  templateUrl: './personalia.component.html',
  styleUrls: ['./personalia.component.css']
})

export class PersonaliaComponent implements OnInit {
  AmountBeacons = 0;
  updateResident: any;
  src2: any;
  tag: any;
  id: string = this.route.snapshot.params['id'];
  resident: Resident;
  countI: string;
  countV: string;
  countX: string;

    /**
     * Injectable
     * @param {RestServiceService} service
     * @param {MediaService} media
     * @param {ActivatedRoute} route
     * @param {Router} router
     * @param {LoginService} login
     */
  constructor(private service: RestServiceService, private media: MediaService, private route: ActivatedRoute, private router: Router,private login:LoginService) {
      this.src2 = "/api/v1/residents/" + this.id + "/picture?token="+this.login.token;
  }

    ngOnInit() {
        this.resident = <Resident>{firstName: '', lastName: '', room: '', id: '', birthday: new Date(), doctor: {name: '', phoneNumber: ''}};
        this.showOneResident();
        this.getImageCount();
    }

  get baseUr() {
    return document.getElementsByTagName('base')[0].href;
  }

    /**
     * Show one resident on the page
     * @returns {Promise<void>}
     */
  async showOneResident() {
    const resident: any = await this.service.getResidentBasedOnId(this.id);
    if (resident !== undefined) {
      this.resident = resident;
    } else {
      this.router.navigate(['/error']);
    }
  }

    /**
     * Delete a residents tag
     * @param tag beaconTagID
     * @returns {Promise<void>}
     */
  async deleteTag(tag: any) {
    await this.service.deleteTagFromResident(this.id, tag);
    this.showOneResident();
    $('#deleteTagModal').modal('close');
  }

    /**
     * Get Media Count, how many urls etc there are to show how much there is for each
     * UnderRevision
     * @returns {Promise<void>} numbers for displaying amount
     */
  async getImageCount() {
    const count = await this.media.getMedia(this.id, '/images');
    const count2 = await this.media.getMedia(this.id, '/videos');
    const count3 = await this.media.getMedia(this.id, '/music');
    this.countV = count2.length;
    this.countI = count.length;
    this.countX = count3.length;
  }

    /**
     * Closes delete modal
     * @constructor
     */
  CloseModal() {
    $('#deleteTagModal').modal('close');
  }

    /**
     * Open modal to delete tag
     * @param tag
     */
  deleteTagModal(tag: any) {
    this.tag = tag;
    $('#deleteTagModal').modal();
    $('#deleteTagModal').modal('open');

  }

    /**
     * Open Add beacon modal only accepts integers
     */
  openAddBeaconModal() {
    $('#add-beacon-modal').modal();
    $('#add-beacon-modal').modal('open');
  }

    /**
     * Reset the form
     * @param {NgForm} form
     */
  resetForm(form: NgForm) {
    form.reset();
  }

    /**
     * Add a beacon to a resident
     * @param {NgForm} form
     * @returns {Promise<void>}
     */
  async addBeaconTag(form: NgForm) {
    const beaconMinorNumber = {
      beaconNumber: form.value.aBeaconMinorNumber
    };

    const bmn = beaconMinorNumber.beaconNumber + '';

    const a = await this.service.addTagToResident(this.resident.id, bmn);
    console.log(a);
    if (a !== undefined) {
      this.resident.tags = a;
    } else {
      Materialize.toast('Oeps! Beacon ' + bmn + ' is al gekoppeld aan een bewoner ', 4000);
    }

    form.reset();
    setTimeout(() => {
      $('#add-beacon-modal').modal('close');
    }, 200);
  }

}
