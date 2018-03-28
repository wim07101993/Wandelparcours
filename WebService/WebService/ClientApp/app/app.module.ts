import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { StationmanagementComponent } from './components/stationmanagement/stationmanagement.component';
import { HeaderComponent } from './components/header/header.component';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { ResidentsComponent } from './components/residents/residents.component';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { SeniorComponent } from './components/senior/senior.component';
import { PictureComponent } from './components/senior/media/picture/picture.component';
import { AudioComponent } from './components/senior/media/audio/audio.component';
import { VideoComponent } from './components/senior/media/video/video.component';
import { GameComponent } from './components/senior/media/game/game.component';
import { TrackingComponent } from './components/senior/tracking/tracking.component';
import { PersonaliaComponent } from './components/senior/personalia/personalia.component';
import { ErrorPageComponent } from './components/error-page/error-page.component';
import { UploadComponent } from './components/senior/upload/upload.component';
import { BrowserModule } from '@angular/platform-browser';
import { GlobaltrackingComponent } from './components/globaltracking/globaltracking.component';

const appRoutes: Routes = [
    { path: '', redirectTo: 'residents', pathMatch: 'full' },
    { path: 'residents', component: ResidentsComponent },
    { path: 'resident/:id', component: PersonaliaComponent },
    { path: 'resident/:id/picture', component: PictureComponent },
    { path: 'resident/:id/video', component: VideoComponent },
    { path: 'resident/:id/audio', component: AudioComponent },
    { path: 'resident/:id/game', component: GameComponent },
    { path: 'resident/:id/tracking', component: TrackingComponent },
    { path: 'counter', component: CounterComponent },
    { path: 'fetch-data', component: FetchDataComponent },
    { path: 'modules', component: StationmanagementComponent },
    { path: 'error', component: ErrorPageComponent},
    { path: 'residents', component: ResidentsComponent },
    { path: 'tracking', component: GlobaltrackingComponent },
    { path: '**', redirectTo: 'residents' }
    
];

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        SidenavComponent,
        HomeComponent,
        HeaderComponent,
        ResidentsComponent,
        StationmanagementComponent,
        SeniorComponent,
        PictureComponent,
        AudioComponent,
        VideoComponent,
        GameComponent,
        TrackingComponent,
        PersonaliaComponent,
        ErrorPageComponent,
        UploadComponent,
        GlobaltrackingComponent,
    ],
    imports: [
        CommonModule,
        Ng2SearchPipeModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot(appRoutes),
        BrowserModule

    ]
})
export class AppModuleShared {
}
