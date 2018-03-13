import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
    @Input() type: string;
    fd: FormData = new FormData();
    selectedFile: File;

    constructor() {}

  ngOnInit() {
  }

  onFileSelected(event: any) {
      this.selectedFile = <File>event.target.files[0];
      console.log(this.selectedFile);
  }

  async onUpload() {
      this.fd.append('images/data', this.selectedFile, this.selectedFile.name);
      console.log(this.fd); console.log(this.selectedFile.type);
      //let headers = new Headers();
      //let options = new RequestOptions({ headers: headers });    
      //await this.service.addImagesToDatabase(this.id, new Blob([JSON.stringify(this.selectedFile.name)], { type: "application/json"}));
      //let images = this.service.addImagesToDatabase(this.id, this.fd);
      //console.log(images);
  }    



}
