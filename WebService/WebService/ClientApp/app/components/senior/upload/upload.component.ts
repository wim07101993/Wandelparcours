import { Component, OnInit, Input } from '@angular/core';
import { RestServiceService } from '../../../service/rest-service.service';
import { ActivatedRoute } from '@angular/router';
import { RequestOptions } from '@angular/http';

declare var $: any;

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
    id: string = this.route.snapshot.params['id'];
    @Input() type: string;
    fd: FormData;
    selectedFile: any;
    loading: string = "";
    images: any;

    constructor(private restService: RestServiceService, private route: ActivatedRoute) {}

  ngOnInit() {
  }

  onFileSelected(event: any) {
      this.loading = "Upload"
      this.selectedFile = <any>event.target.files;   
  }

  async onUpload() {
      for (const file in this.selectedFile) {
          const index = parseInt(file);
          if (!isNaN(index)) {
              const fd = new FormData();
              //console.log(this.selectedFile[index]);
              fd.append("File", this.selectedFile[index], this.selectedFile[index].name);
              console.log(fd);
              this.images = await this.restService.addImagesToDatabase(this.id, fd);
              
          }
          
      }
      if (this.images) {
          this.loading = "Uploaded!"
      }
      

      this.selectedFile = [];


  }    



}
