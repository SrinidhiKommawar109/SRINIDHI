import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface NotificationMessage {
  id: string;
  title: string;
  detail?: string;
  type: 'success' | 'error' | 'info';
}

@Injectable({
  providedIn: 'root',
})
export class NotificationsService {
  private readonly messagesSubject = new BehaviorSubject<NotificationMessage[]>([]);
  readonly messages$ = this.messagesSubject.asObservable();

  show(message: Omit<NotificationMessage, 'id'>, ttlMs = 3500): void {
    const id =
      typeof crypto !== 'undefined' && 'randomUUID' in crypto
        ? crypto.randomUUID()
        : Date.now().toString();
    const next = [...this.messagesSubject.value, { ...message, id }];
    this.messagesSubject.next(next);

    setTimeout(() => {
      this.dismiss(id);
    }, ttlMs);
  }

  dismiss(id: string): void {
    const next = this.messagesSubject.value.filter((msg) => msg.id !== id);
    this.messagesSubject.next(next);
  }
}

