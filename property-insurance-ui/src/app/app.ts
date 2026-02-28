import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ThemeService } from './core/theme.service';
import { NotificationsComponent } from './shared/notifications.component';
import { AuthService } from './core/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NotificationsComponent, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('property-insurance-ui');
  private readonly themeService = inject(ThemeService);
  private readonly authService = inject(AuthService);
  themeMode = this.themeService.getCurrent();
  isLoggedIn = signal(this.authService.isLoggedIn());

  constructor() {
    this.themeService.apply(this.themeMode);
    this.authService.authState$.subscribe((state) => {
      this.isLoggedIn.set(state);
    });
  }

  toggleTheme(): void {
    this.themeMode = this.themeService.toggle();
  }

  logout(): void {
    this.authService.logout();
  }
}
