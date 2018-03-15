import { Component, OnInit, Input } from '@angular/core';
import { RestServiceService } from '../../../service/rest-service.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
    id: string = this.route.snapshot.params['id'];
    @Input() type: string;
    fd: FormData = new FormData();
    selectedFile: File;
    blob: Blob;

    constructor(private restService: RestServiceService, private route: ActivatedRoute) {}

  ngOnInit() {
  }

  onFileSelected(event: any) {
      this.selectedFile = <File>event.target.files[0];
      console.log(this.selectedFile);
  }

  async onUpload() {
      this.fd.append('images/data', this.selectedFile, this.selectedFile.name);
      this.blob = new Blob()
      console.log(this.fd);
      //let headers = new Headers();
      //let options = new RequestOptions({ headers: headers });    
      //await this.service.addImagesToDatabase(this.id, new Blob([JSON.stringify(this.selectedFile.name)], { type: "application/json"}));
      let images = this.restService.addImagesToDatabase(this.id, this.fd);
      //console.log(images);
  }    



}
