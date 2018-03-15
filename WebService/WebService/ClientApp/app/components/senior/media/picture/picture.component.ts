import { Component, OnInit, ViewChild, Injectable, Input } from '@angular/core';
import { RestServiceService } from '../../../../service/rest-service.service';
import { ActivatedRoute } from '@angular/router';
import { RequestOptions, Headers } from '@angular/http';
import { UploadComponent } from '../../upload/upload.component';
declare var $: any;


@Component({
  selector: 'app-picture',
  templateUrl: './picture.component.html',
  styleUrls: ['./picture.component.css'],
})
export class PictureComponent implements OnInit {
    typeOfMedia: string;
    ngOnInit() { }


    id: string = this.route.snapshot.params['id'];
    selectedFile: File;

    constructor(private service: RestServiceService, private route: ActivatedRoute, ) {
        this.getAllImages();
        this.typeOfMedia = "image/*";
    }

    async getAllImages() {
        let images: any = await this.service.getImagesOfResidentBasedOnId(this.id);
        console.log(images);
    }
}