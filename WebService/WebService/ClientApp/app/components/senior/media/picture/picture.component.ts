import { Component, OnInit, ViewChild, Injectable } from '@angular/core';
import { RestServiceService } from '../../../../service/rest-service.service';
import { ActivatedRoute } from '@angular/router';
declare var $: any;


@Component({
  selector: 'app-picture',
  templateUrl: './picture.component.html',
  styleUrls: ['./picture.component.css'],
})
export class PictureComponent implements OnInit {
    ngOnInit() { }


    id: string = this.route.snapshot.params['id'];
    selectedFile: File;

    constructor(private service: RestServiceService, private route: ActivatedRoute, ) {
      
    }

    onFileSelected(event: any) {
        this.selectedFile = <File>event.target.files[0];
    }

    async onUpload() {
        const fd = new FormData();
        fd.append('image', this.selectedFile, this.selectedFile.name);

        let images = await this.service.addImagesToDatabase(this.id, fd);
        console.log(images);
    }
    
    

    async getAllImages() {
        let images: any = await this.service.getImagesOfResidentBasedOnId(this.id);
        console.log(images);
    }
}