import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LogInComponent } from './log-in/log-in.component';
import { RegistrationComponent } from './registration/registration.component';
import { AdminSerialsComponent } from './admin-serials/admin-serials.component';
import { AdminUsersComponent } from './admin-users/admin-users.component';
import { ResultsComponent } from './results/results.component';
import { NewSerialComponent } from './new-serial/new-serial.component';
import { AdminChaptersComponent } from './admin-chapters/admin-chapters.component';
import { authGuard } from './guard/authen.guard';
import { adminGuard } from './guard/admin.guard';
import { AdminNewserialComponent } from './admin-newserial/admin-newserial.component';
import { LikedSerialsComponent } from './liked-serials/liked-serials.component';
import { PasswordResetComponent } from './password-reset/password-reset.component';
import { SerialDetailsComponent } from './serial-details/serial-details.component';
import { ChangePasswordComponent } from './change-password/change-password.component';


//TODO delete, and fix
const routes: Routes = [
  {path: 'home', component: HomeComponent},
  {path: 'login', component: LogInComponent},
  {path: 'register', component: RegistrationComponent},
  {path: 'adminSerials', canActivate: [authGuard, adminGuard], component: AdminSerialsComponent},
  // {path: 'adminSerials', component: AdminSerialsComponent},
  {path: 'adminUsers', canActivate: [authGuard, adminGuard], component: AdminUsersComponent},
  // {path: 'adminUsers', component: AdminUsersComponent},
  {path: 'adminChapters', canActivate: [authGuard, adminGuard], component: AdminChaptersComponent},
  // {path: 'adminChapters', component: AdminChaptersComponent},
  {path: 'results', component: ResultsComponent},
  {path: 'likedSer', component: LikedSerialsComponent},
  // {path: 'search', component: SearchComponent},
  {path: 'newSerial', canActivate: [authGuard], component: NewSerialComponent},
  // {path: 'newSerial', component: NewSerialComponent},
  {path: 'adminNewSerial', canActivate: [authGuard, adminGuard], component: AdminNewserialComponent},
  // {path: 'adminNewSerial', component: AdminNewserialComponent},
  {path: 'resetPassword', component: PasswordResetComponent},
  {path: 'changePassword', component: ChangePasswordComponent},
  {path: 'serialDetails/:id', component: SerialDetailsComponent},
  {path: '', component: HomeComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation : 'reload' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
