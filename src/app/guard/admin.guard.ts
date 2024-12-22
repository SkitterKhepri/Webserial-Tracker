import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthServiceService } from '../services/auth-service.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authServ = inject(AuthServiceService)

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
