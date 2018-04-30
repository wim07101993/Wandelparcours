import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {RouterModule, Routes} from '@angular/router';
import { LoginService } from "./service/login-service.service";
import { StoreService } from "./service/store.service";

import { AppComponent } from './components/app/app.component';
import { StationmanagementComponent } from './components/stationmanagement/stationmanagement.component';
import { HeaderComponent } from './components/header/header.component';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { ResidentsComponent } from './components/residents/residents.component';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { SeniorComponent } from './components/senior/senior.component';
import { PersonaliaComponent } from './components/senior/personalia/personalia.component';
import { AudioComponent } from './components/senior/media/audio/audio.component';
import { GameComponent } from './components/senior/media/game/game.component';
import { PictureComponent } from './components/senior/media/picture/picture.component';
import { VideoComponent } from './components/senior/media/video/video.component';
import { UploadComponent } from './components/senior/upload/upload.component';
import { TrackingComponent } from './components/senior/tracking/tracking.component';
import { GlobaltrackingComponent } from './components/globaltracking/globaltracking.component';
import { LoginComponent } from './components/login/login.component';
import { UsersComponent } from './components/users/users.component';

const appRoutes: Routes = [
      { path: '', redirectTo: 'residents', pathMatch: 'full' },
      { path: 'login', component: LoginComponent },
      { path: 'residents', component: ResidentsComponent },
      { path: 'resident/:id', component: PersonaliaComponent },
      { path: 'resident/:id/picture', component: PictureComponent },
      { path: 'resident/:id/video', component: VideoComponent },
      { path: 'resident/:id/audio', component: AudioComponent },
      { path: 'resident/:id/game', component: GameComponent },
      { path: 'resident/:id/tracking', component: TrackingComponent },
      { path: 'modules', component: StationmanagementComponent },
      { path: 'tracking', component: GlobaltrackingComponent },
      { path: 'users', component: UsersComponent },
      { path: '**', redirectTo: 'residents' }
];

@NgModule({
  declarations: [
    AppComponent,
    StationmanagementComponent,
    HeaderComponent,
    SidenavComponent,
    ResidentsComponent,
    SeniorComponent,
    PersonaliaComponent,
    AudioComponent,
    GameComponent,
    PictureComponent,
    VideoComponent,
    UploadComponent,
    TrackingComponent,
    GlobaltrackingComponent,
    LoginComponent,
    UsersComponent,
  ],
  imports: [
    CommonModule,
    Ng2SearchPipeModule,
    FormsModule,
    RouterModule.forRoot(appRoutes),
    BrowserModule
  ],
  providers: [LoginService, StoreService],
  bootstrap: [AppComponent]
})
export class AppModuleShared {
}
