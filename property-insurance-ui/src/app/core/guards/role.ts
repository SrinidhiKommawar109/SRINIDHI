import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const role = (allowedRoles: string[]): CanActivateFn => {
    return () => {

        const router = inject(Router);
        const role = localStorage.getItem('role');

        if (role && allowedRoles.includes(role)) {
            return true;
        }

        router.navigate(['/']);
        return false;
    };
};