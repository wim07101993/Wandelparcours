import { Component, OnInit, ViewChild, Injectable, Input } from '@angular/core';
import { RestServiceService } from '../../../../service/rest-service.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestOptions, Headers } from '@angular/http';
import { UploadComponent } from '../../upload/upload.component';
import { Resident } from '../../../../models/resident';
import { MediaService } from '../../../../service/media.service';
declare var $: any;


@Component({
    selector: 'app-picture',
    templateUrl: './picture.component.html',
    styleUrls: ['./picture.component.css'],
})
export class PictureComponent implements OnInit {
    typeOfMedia: string;
    picture: string = "/images";
    check: any;
    ngOnInit() { }

    deleteResidentImage: Resident;
    images: Resident[];
    fullLinks: any=[];
    id: string = this.route.snapshot.params['id'];
    selectedFile: File;

    constructor(private route: ActivatedRoute, private router: Router, private media: MediaService) {
        this.getAllImages();
        this.typeOfMedia = "image/*";
        this.deleteResidentImage = <Resident>{images: {}};
    }

    /**
     * reload page
     */
    reload() {
        this.getAllImages();
    }

    /**
     * Gets all urls for images
     */
    async getAllImages() {
        this.fullLinks = [];
        this.fullLinks = await this.media.getMedia(this.id, this.picture);
        //console.log(this.fullLinks);
    }

    /**
     * Delete resident media based on uniqueId
     * @param uniquePictureID unique pictureId
     * Either reloads the page or sends user to errorpage
     */
    async deleteResidentMediaByUniqueId(uniquePictureID: string) {
        this.check = await this.media.deleteMedia(this.id, uniquePictureID, this.picture);
        if (this.check) {
            this.getAllImages();
        } else {
            this.router.navigate(["/error"]);
        }
        $("#deleteModal").modal("close");
    }

    /*
    *   Closes the modal to delete a picture 
    */
    CloseModal() {
        $("#deleteModal").modal("close");
    }

    /*
    *   Opens modal to delete a picture
    */
    deleteModal(resident: Resident) {
        
        this.deleteResidentImage = resident;
        console.log(resident.images.id);
            // noinspection JSJQueryEfficiency
            $("#deleteModal").modal();
            // noinspection JSJQueryEfficiency
            $("#deleteModal").modal("open");
            
    }

}