import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth-service.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authServ = inject(AuthService)

  if(authServ.getCurrentClaims() == null){
    authServ.noAdminClaims()
    return false
  }

  console.log("admin guard")
  if(authServ.getCurrentClaims().includes('Admin') || authServ.getCurrentClaims().includes('SAdmin')){
    return true
  }
  authServ.noAdminClaims()
  return false
};
