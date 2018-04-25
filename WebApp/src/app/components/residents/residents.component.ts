import {Component, OnInit, ViewChild} from '@angular/core';
import {Resident} from '../../models/resident';
import {RestServiceService} from '../../service/rest-service.service';
import {NgForm} from '@angular/forms';
import {Router} from '@angular/router';


declare var $: any;
declare var Materialize: any;

@Component({
  selector: 'app-residents',
  templateUrl: './residents.component.html',
  styleUrls: ['./residents.component.css'],
  providers: [RestServiceService]
})

export class ResidentsComponent implements OnInit {
  view = 'card-view';
  term: any = null;
  residents: any = [];
  modalResident: Resident;
  updateResident: any;
  search = false;
  profilePic: any;
  selectedFile: any = [];
  reader: any;
  selectedFileImage: any = [];
  fd: any;
  @ViewChild('myInput') myInputVariable: any;

  /**
   * Injects the service and router
   * @param service Restservice
   * @param router Router
   */
  constructor(private service: RestServiceService, private router: Router) {

  }

    ngOnInit(): void {
        this.showAllResidents();
        this.residents = [];
        this.profilePic = [];

        /*Creates empty modals to avoid collision with previous existing modals should they not be deleted*/
        this.modalResident = <Resident>{
            firstName: '',
            lastName: '',
            room: '',
            id: '',
            birthday: new Date(),
            doctor: {name: '', phoneNumber: ''},
            pictureUrl: ''
        };
        this.updateResident = {
            firstName: '', lastName: '', room: '', id: '', birthday: '', doctor: {name: '', phoneNumber: ''}
        };

    }

  /**
   * Close modal
   */

  closeModal() {
    $().modal('close');
  }



  /**
   * Focus to input for the searchbar --> input will be active if event 'button' has been pressed
   */
  focusInput() {
    setTimeout(() => {
      $('#focusToInput').focus();
    }, 200);

  }

  /**
   * Observer event if anything changes
   * @param event
   */
  onFileSelected(event: any) {
    // this.loading = "Upload"
    this.selectedFile = <any>event.target.files;
    this.reader = new FileReader();
    this.reader.readAsDataURL(event.target.files[0]);
    this.reader.onload = (nt: any) => {
      this.profilePic = nt.target.result;
    };

  }

  /**
   * Opens modal
   * @param modalResident
   */
  openModal(modalResident: Resident) {
    // alert(uniqueIdentifier);
    this.modalResident = modalResident;
    $('#deleteModalResident').modal();
    $('#deleteModalResident').modal('open');
  }

  /**
   *
   * Open modal in edit mode and fill modal with resident
   * @param modalResident
   *
   */
  openEditModal(modalResident: Resident) {
    this.modalResident = modalResident;
    console.log(this.modalResident);
    $('#editModalResident').modal();
    $('#editModalResident').modal('open');
    $('.datepicker').pickadate(
      {
        clearDate: true,
        monthsFull: ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni', 'Juli',
          'Agustus', 'September', 'Oktober', 'November', 'December'],
        monthsShort: ['Jan', 'Feb', 'Maa', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
        weekdaysShort: ['ma', 'di', 'wo', 'do', 'vr', 'zat', 'zon'],
        selectMonths: true, // Creates a dropdown to control month
        selectYears: 120, // Creates a dropdown of 15 years to control year,
        max: new Date(),
        today: 'Vandaag',
        clear: 'Wissen',
        close: 'Bevestigen',
        closeOnSelect: true,
        format: 'mm-dd-yyyy'
      });
  }

  /**
   * get all residents async from service
   */
  async showAllResidents() {
    const residents: any = await this.service.getAllResidents();
    console.log(residents);
    if (residents !== undefined) {
      this.residents = residents;
    } else {
      this.router.navigate(['/error']);
    }
  }


  /**
   * Delete resident async based on unique identifier --> "ID"
   * @param uniqueIdentifier
   */
  async deleteResident(uniqueIdentifier: string) {
    await this.service.deleteResidentByUniqueId(uniqueIdentifier);
    this.showAllResidents();
  }

  /**
   * Edit and save resident from service
   * @param resident of type Resident
   */

  async editResident(resident: Resident) {
    // get correct ID
    this.updateResident.id = resident.id;
    const birthDay = $('#birthDay').val();

    console.log(birthDay);

    // if birthday hasn't been entered make sure birthday is of type Date
    if (birthDay !== '') {
      // console.log("update birthday");
      const a = new Date(birthDay);
      console.log(a);
      this.updateResident.birthday = a;
    }
    /**
     * Send resident object and the changed properties
     */
    const changedProperties = [];
    for (const prop in this.updateResident) {
      if (this.updateResident[prop] != null && this.updateResident[prop] !== '') {
        changedProperties.push(prop);
      }
    }
    if (this.updateResident.doctor.name === '') {
      this.updateResident.doctor.name = this.modalResident.doctor.name;
    }
    if (this.updateResident.doctor.phoneNumber === '') {
      this.updateResident.doctor.phoneNumber = this.modalResident.doctor.phoneNumber;
    }
    if (this.updateResident.birthday === '') {
      this.updateResident.birthday = this.modalResident.birthday;
    }
    console.log(changedProperties);

    $('#birthDay').val('');

    const updateData = this.updateResident;
    console.log('test');


    for(const file in this.selectedFile) {
      try {
        const index = parseInt(file);
        if (!isNaN(index)) {
          // this.loading = "uploading...";
          const fd = new FormData();
          fd.append('File', this.selectedFile[index], this.selectedFile[index].name);
          if (this.selectedFile[index].type.indexOf('image') != -1) {
            await this.service.addProfilePic(this.updateResident.id, fd);
          } else {
            alert('won\'t work');
          }

        }
      } catch (e) {
        console.log(e);
      }
    }
    await this.service.editResidentWithData(updateData, changedProperties);
    this.updateResident = {
      firstName: '', lastName: '', room: '', id: '', birthday: '', doctor: {name: '', phoneNumber: ''}
    };

    this.reset();

    setTimeout(() => {
      $('#editModalResident').modal('close');
    }, 200);
    // get all residents again after updating
    this.showAllResidents();
  }

  /**
   * Add resident to database
   * @param form of type NgForm
   */
  async addResident(form: NgForm) {
    // Workaround for dateformat
    const birthday = $('#abirthdate').val();
    let a;

    if (birthday !== '') {
      a = new Date(birthday);
    }

    for (const file in this.selectedFile) {
      const index = parseInt(file);
      if (!isNaN(index)) {
        // this.loading = "uploading...";
        this.fd = new FormData();
        this.fd.append('File', this.selectedFile[index], this.selectedFile[index].name);
        if (this.selectedFile[index].type.indexOf('image') !== -1) {
          this.selectedFileImage = this.selectedFile[index];
          //this.check = await this.restService.addCorrectMediaToDatabase(this.id, fd, this.addPicture);
        }
      }
    }

    // Get data from form-inputs
    const data = {
      firstName: form.value.aFirstName,
      lastName: form.value.aLastName,
      room: form.value.aRoom,
      birthday: a,
      doctor: {name: form.value.aDoctor, phoneNumber: form.value.aTelefoon},
    };
    console.log(this.fd);

    // Send gathered data over the resrService
    const id = await this.service.addResident(data);
    if (id !== undefined) {
      await this.service.addProfilePic(id, this.fd);
      Materialize.toast(`bewoner: ${data.firstName} ${data.lastName} succesvol toegevoegd`, 5000);
    } else {
      Materialize.toast(`Bewoner kon niet worden toegevoegd!`, 5000);
      this.router.navigate(['/error']);
    }

    // Reset Residents form
    form.reset();
    this.reset();
    // close modal/form and 'reload' page
    setTimeout(() => {
      $('#add-resident-modal').modal('close');
    }, 200);

    this.showAllResidents();

  }

  /**
   * Reset the form on close
   * @param form of type NgForm
   */
  resetForm(form: NgForm) {
    form.reset();
  }

  openResidentAddModal() {

    $('#add-resident-modal').modal();
    $('#add-resident-modal').modal('open');
    $('.datepicker').pickadate({
      selectMonths: true, // Creates a dropdown to control month
      selectYears: 110, // Creates a dropdown of 15 years to control year,
      max: new Date(),
      monthsFull: ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni', 'Juli',
        'Agustus', 'September', 'Oktober', 'November', 'December'],
      monthsShort: ['Jan', 'Feb', 'Maa', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
      weekdaysShort: ['ma', 'di', 'wo', 'do', 'vr', 'zat', 'zon'],
      today: 'Vandaag',
      clear: 'Wissen',
      close: 'Ok',
      formatSubmit: 'mm-dd-yyyy',
      dateFormat: 'mm-dd-yyyy',
      format: 'mm-dd-yyyy', // hier loopt iets mis?
      hiddenName: true,
      closeOnSelect: true // Close upon selecting a date,
    });
  }

  reset() {
    console.log(this.myInputVariable.nativeElement.files);
    this.myInputVariable.nativeElement.value = '';
    // this.myInputImage.nativeElement.value = "Geen bestand geselecteerd";
    this.profilePic = '';
    console.log(this.myInputVariable.nativeElement.files);
  }

  /**
   * Navigate to deffrent page with Object resident (only ID is enough) --> this will set the URL for that resident which makes it possible
   * to Get said resident
   * @param resident of type Resident
   */
  navigateTo(resident: Resident) {
    // console.log(resident.id);
    this.router.navigate(['/resident/' + resident.id]);
  }
}
