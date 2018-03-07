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
import { ResidentComponent } from './components/resident/resident.component';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { MediaComponent } from './components/media/media.component';
//import { RestServiceService } from './service/rest-service.service';

const appRoutes: Routes = [
    { path: '', redirectTo: 'resident', pathMatch: 'full' },
    { path: 'resident', component: ResidentComponent },
    { path: 'resident/:id', component: MediaComponent },
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
        MediaComponent,
    ],
    imports: [
        CommonModule,
        Ng2SearchPipeModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot(appRoutes)
    ]
    //providers: [RestServiceService]
})
export class AppModuleShared {
}
