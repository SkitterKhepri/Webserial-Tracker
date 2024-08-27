import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LogInComponent } from './log-in/log-in.component';
import { RegistrationComponent } from './registration/registration.component';
import { AdminSerialsComponent } from './admin-serials/admin-serials.component';
import { AdminUsersComponent } from './admin-users/admin-users.component';
import { ResultsComponent } from './results/results.component';
import { SearchComponent } from './search/search.component';

const routes: Routes = [
  {path: 'home', component: HomeComponent},
  {path: 'login', component: LogInComponent},
  {path: 'register', component: RegistrationComponent},
  {path: 'adminSerials', component: AdminSerialsComponent},
  {path: 'adminUsers', component: AdminUsersComponent},
  {path: 'results', component: ResultsComponent},
  {path: 'search', component: SearchComponent},
  {path: '', component: HomeComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
