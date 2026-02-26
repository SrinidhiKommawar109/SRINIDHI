import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html'
})
export class Login {

  email = '';
  password = '';

  constructor(private auth: AuthService, private router: Router) { }

  login() {
    this.auth.login(this.email, this.password).subscribe((res: any) => {

      localStorage.setItem('token', res.token);
      localStorage.setItem('role', res.role);

      if (res.role === 'Admin') {
        this.router.navigate(['/admin']);
      }
    });
  }
}