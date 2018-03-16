import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
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
    check: any;
    @Output() reload = new EventEmitter();

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
                  this.loading = "uploading...";
                  const fd = new FormData();
                  fd.append("File", this.selectedFile[index], this.selectedFile[index].name);
                  if (this.selectedFile[index].type.indexOf("image") != -1) {
                      this.check = await this.restService.addImagesToDatabase(this.id, fd);
                      //console.log(this.selectedFile[index].name);
                  }
                  else if (this.selectedFile[index].type.indexOf("video") != -1) {
                      this.check = await this.restService.addVideosToDatabase(this.id, fd);
                  }
                  else{
                      alert("won't work");
                  }
              
              }
          
          }
          this.selectedFile = [];
          if (this.check) {
              //console.log(this.reload);
              this.reload.emit();
              this.loading = "Uploaded!"
          }

          
      }    



}
