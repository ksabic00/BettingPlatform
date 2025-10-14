import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface Toast {
  id: string;
  type: ToastType;
  text: string;
  createdAt: number;
  duration: number; 
}

function guid() {
  return Math.random().toString(36).slice(2) + Date.now().toString(36);
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly _toasts = new BehaviorSubject<Toast[]>([]);
  readonly toasts$ = this._toasts.asObservable();

  show(type: ToastType, text: string, duration = 5000) {
    const t: Toast = { id: guid(), type, text, createdAt: Date.now(), duration };
    const list = [...this._toasts.value, t];
    this._toasts.next(list);

    setTimeout(() => this.dismiss(t.id), duration);
  }

  success(text: string, duration?: number) { this.show('success', text, duration); }
  error(text: string, duration?: number)   { this.show('error', text, duration); }
  info(text: string, duration?: number)    { this.show('info', text, duration); }
  warning(text: string, duration?: number) { this.show('warning', text, duration); }

  dismiss(id: string) {
    this._toasts.next(this._toasts.value.filter(t => t.id !== id));
  }

  clear() { this._toasts.next([]); }
}
