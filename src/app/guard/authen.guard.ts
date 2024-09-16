import { CanActivateFn, Router } from '@angular/router';
import { AuthServiceService } from '../services/auth-service.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthServiceService);

  if (authService.isSessionExpired()) {
    authService.logoutGoLogin()
    return false;
  }
  return true;
};
