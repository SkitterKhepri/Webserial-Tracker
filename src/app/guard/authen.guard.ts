import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth-service.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  if (authService.isSessionExpired()) {
    // console.log("guard")
    authService.logoutGoLogin()
    return false;
  }
  return true;
};
