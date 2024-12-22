import { CanActivateFn, Router } from '@angular/router';
import { AuthServiceService } from '../services/auth-service.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthServiceService);

  if (authService.isSessionExpired()) {
    console.log("guard")
    authService.logoutGoLogin()
    return false;
  }
  return true;
};
