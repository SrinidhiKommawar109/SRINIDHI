import { Injectable } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly storageKey = 'pis_theme';

  getCurrent(): ThemeMode {
    const saved = localStorage.getItem(this.storageKey) as ThemeMode | null;
    return saved === 'light' || saved === 'dark' ? saved : 'dark';
  }

  apply(mode: ThemeMode): void {
    const root = document.documentElement;
    if (mode === 'dark') {
      root.classList.add('dark');
    } else {
      root.classList.remove('dark');
    }
    localStorage.setItem(this.storageKey, mode);
  }

  toggle(): ThemeMode {
    const next = this.getCurrent() === 'dark' ? 'light' : 'dark';
    this.apply(next);
    return next;
  }
}

