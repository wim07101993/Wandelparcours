import { Component, OnInit, ViewChild, Injectable, Input } from '@angular/core';
import { RestServiceService } from '../../../../service/rest-service.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestOptions, Headers } from '@angular/http';
import { UploadComponent } from '../../upload/upload.component';
import { Resident } from '../../../../models/resident';
declare var $: any;


@Component({
    selector: 'app-picture',
    templateUrl: './picture.component.html',
    styleUrls: ['./picture.component.css'],
})
export class PictureComponent implements OnInit {
    typeOfMedia: string;
    ngOnInit() { }



    images: Resident[];
    fullLinks: any=[];
    url: any = "http://localhost:5000/api/v1/media/";
    id: string = this.route.snapshot.params['id'];
    selectedFile: File;

    constructor(private service: RestServiceService, private route: ActivatedRoute, private router: Router) {
        this.getAllImages();
        this.typeOfMedia = "image/*";
    }

    reload() {
        this.getAllImages();
    }

    async getAllImages() {
        this.fullLinks = [];
        this.images = await this.service.getImagesOfResidentBasedOnId(this.id);
        for (let a of this.images){
            let url2 = this.url + a.id;
            let fullLinks = new Resident();
            
            fullLinks.images.id = a.id;
            fullLinks.images.url = url2; 
            this.fullLinks.push(fullLinks);
        }
    }


    async deleteImageOnId(uniquePictureID: string) {
        let a: any = await this.service.deleteResidentImageByUniqueId(this.id, uniquePictureID);
        if (a) {
            this.getAllImages();
        } else {
            this.router.navigate(["/error"]);
        }
    }
}