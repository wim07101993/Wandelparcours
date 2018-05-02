import {Component, OnInit} from '@angular/core';
import {Resident} from '../../../models/resident';
import {ActivatedRoute, Router} from '@angular/router';
import {RestServiceService} from '../../../service/rest-service.service';
import {MediaService} from '../../../service/media.service';
import {NgForm} from '@angular/forms';

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
  tag: any;
  id: string = this.route.snapshot.params['id'];
  resident: Resident;
  countI = 0;
  countV = 0;

  constructor(private service: RestServiceService, private media: MediaService, private route: ActivatedRoute, private router: Router) {}

    ngOnInit() {
        this.resident = <Resident>{firstName: '', lastName: '', room: '', id: '', birthday: new Date(), doctor: {name: '', phoneNumber: ''}};
        this.showOneResident();
        this.getImageCount();
    }

  get baseUr() {
    return document.getElementsByTagName('base')[0].href;
  }

  async showOneResident() {
    const resident: any = await this.service.getResidentBasedOnId(this.id);
    if (resident !== undefined) {
      this.resident = resident;
    } else {
      this.router.navigate(['/error']);
    }
  }

  async deleteTag(tag: any) {
    await this.service.deleteTagFromResident(this.id, tag);
    this.showOneResident();
    $('#deleteTagModal').modal('close');
  }

  async getImageCount() {
    const count = await this.media.getMedia(this.id, '/images');
    const count2 = await this.media.getMedia(this.id, '/videos');
    this.countV = count2.length;
    this.countI = count.length;
  }

  /*
  *   Closes the modal to delete a tag/beacon
  */
  CloseModal() {
    $('#deleteTagModal').modal('close');
  }

  /*
  *   Opens modal to delete a tag
  */
  deleteTagModal(tag: any) {
    this.tag = tag;
    $('#deleteTagModal').modal();
    $('#deleteTagModal').modal('open');

  }

  openAddBeaconModal() {
    $('#add-beacon-modal').modal();
    $('#add-beacon-modal').modal('open');
  }

  /**
   * Reset the form on close
   * @param form of type NgForm
   */
  resetForm(form: NgForm) {
    form.reset();
  }

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
