import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';

import { FileUploadModule } from 'ng2-file-upload';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { StationmanagementComponent } from './components/stationmanagement/stationmanagement.component';
import { HeaderComponent } from './components/header/header.component';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { ResidentComponent } from './components/resident/resident.component';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { SeniorComponent } from './components/senior/senior.component';
import { PictureComponent } from './components/senior/media/picture/picture.component';
import { AudioComponent } from './components/senior/media/audio/audio.component';
import { VideoComponent } from './components/senior/media/video/video.component';
import { GameComponent } from './components/senior/media/game/game.component';
import { TrackingComponent } from './components/senior/tracking/tracking.component';
import { PersonaliaComponent } from './components/senior/personalia/personalia.component';

const appRoutes: Routes = [
    { path: '', redirectTo: 'resident', pathMatch: 'full' },
    { path: 'resident', component: ResidentComponent },
    { path: 'resident/:id', component: PersonaliaComponent },
    { path: 'resident/:id/picture', component: PictureComponent },
    { path: 'resident/:id/video', component: VideoComponent },
    { path: 'resident/:id/audio', component: AudioComponent },
    { path: 'resident/:id/game', component: GameComponent },
    { path: 'resident/:id/tracking', component: TrackingComponent },
    { path: 'counter', component: CounterComponent },
    { path: 'fetch-data', component: FetchDataComponent },
    { path: 'stationmanagement', component: StationmanagementComponent },
    { path: '**', redirectTo: 'resident' },
    { path: 'resident', component: ResidentComponent },
    { path: '**', redirectTo: 'home' }
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
        ResidentComponent,
        StationmanagementComponent,
        SeniorComponent,
        PictureComponent,
        AudioComponent,
        VideoComponent,
        GameComponent,
        TrackingComponent,
        PersonaliaComponent,
    ],
    imports: [
        CommonModule,
        Ng2SearchPipeModule,
        HttpModule,
        FormsModule,
        FileUploadModule,
        RouterModule.forRoot(appRoutes),
    ]
})
export class AppModuleShared {
}
