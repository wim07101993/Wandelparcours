import { Component, OnInit } from '@angular/core';
import { Resident } from '../../../models/resident';
import { Router, ActivatedRoute } from '@angular/router';
import { RestServiceService } from '../../../service/rest-service.service';
import { MediaService } from '../../../service/media.service';
declare var $: any;

@Component({
  selector: 'app-personalia',
  templateUrl: './personalia.component.html',
  styleUrls: ['./personalia.component.css']
})
export class PersonaliaComponent implements OnInit {
    ngOnInit() { }
    updateResident: any;
    tag: any;
    id: string = this.route.snapshot.params['id'];
    resident: Resident;
    countI: number = 0;
    countV: number = 0;
    //router: Router;

    constructor(private service: RestServiceService, private media: MediaService, private route: ActivatedRoute, private router: Router) {
        this.resident = <Resident>{ firstName: "", lastName: "", room: "", id: "", birthday: new Date(), doctor: { name: "", phoneNumber: "" } };
        this.showOneResident();
        this.getImageCount();
        //this.getVideoCount();
    }

    async showOneResident() {
        let resident: any = await this.service.getResidentBasedOnId(this.id);
        console.log(resident);
        if (resident != undefined)
            this.resident = resident;
        else {
            this.router.navigate(['/error']);
        }
    }

    async deleteTag(tag: any) {
        console.log(tag);
        await this.service.deleteTagFromResident(this.id, tag);
        this.showOneResident();
        $("#deleteTagModal").modal("close");
    }

    async getImageCount() {
        let count = await this.media.getMedia(this.id, "/images");
        let count2 = await this.media.getMedia(this.id, "/videos");
        this.countV = count2.length
        this.countI = count.length;
    }

    /*async getVideoCount() {
        let count2 = await this.media.getMedia(this.id, "/videos");
        this.countV = count2.length;
    }*/

    async addTag() {
        this.resident.tags = await this.service.addTagToResident(this.resident.id);
    }


    /*
    *   Closes the modal to delete a tag/beacon
    */
    CloseModal() {
        $("#deleteTagModal").modal("close");
    }

    /*
    *   Opens modal to delete a tag 
    */
    deleteTagModal(tag: any) {

        this.tag = tag;
        //console.log(resident.images.id);
        
        
        // noinspection JSJQueryEfficiency
        console.log(tag);
        $("#deleteTagModal").modal();
        // noinspection JSJQueryEfficiency
        $("#deleteTagModal").modal("open");

    }

}
