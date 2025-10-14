import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from './toast.service';

@Component({
  standalone: true,
  selector: 'app-toast-container',
  imports: [CommonModule],
  template: `
  <div class="toast-wrap">
    <div
      class="toast"
      *ngFor="let t of svc.toasts$ | async"
      [ngClass]="t.type"
      [style.--dur]="t.duration + 'ms'"
      role="alert" aria-live="assertive"
    >
      <div class="text">{{ t.text }}</div>
      <button class="close" (click)="svc.dismiss(t.id)" aria-label="Dismiss">×</button>
      <div class="bar"></div>
    </div>
  </div>
  `,
  styles: [`
    .toast-wrap{
      position:fixed; right:24px; bottom:24px; z-index:9999;
      display:flex; flex-direction:column-reverse; gap:10px; align-items:flex-end;
    }
    .toast{
      min-width:320px; max-width:520px;
      display:flex; align-items:flex-start; gap:12px;
      padding:14px 16px; border-radius:12px; background:#fff;
      box-shadow:0 12px 32px rgba(0,0,0,.16), 0 2px 8px rgba(0,0,0,.06);
      border-left:15px solid transparent;
      font-size:20px; line-height:1.35; color:#0f172a; position:relative;
      transform:translateY(8px); opacity:0; animation:slide-in .22s ease-out forwards;
    }
    .toast.success{ border-left-color:#10b981; background:linear-gradient(0deg,rgba(16,185,129,.08),rgba(16,185,129,.08)), #fff; }
    .toast.error  { border-left-color:#ef4444; background:linear-gradient(0deg,rgba(239,68,68,.08),rgba(239,68,68,.08)), #fff; }
    .toast.info   { border-left-color:#3b82f6; background:linear-gradient(0deg,rgba(59,130,246,.08),rgba(59,130,246,.08)), #fff; }
    .toast.warning{ border-left-color:#f59e0b; background:linear-gradient(0deg,rgba(245,158,11,.10),rgba(245,158,11,.10)), #fff; }

    .text{ flex:1; }
    .close{
      border:0; background:transparent; cursor:pointer; opacity:.6;
      font-size:18px; line-height:1; padding:2px 4px; margin-left:4px;
    }
    .close:hover{ opacity:1; }

    .toast .bar{
      position:absolute; left:0; right:0; bottom:0; height:3px;
      background:transparent; overflow:hidden; border-radius:0 0 12px 12px;
    }
    .toast.success .bar::after{ background:#10b981; }
    .toast.error   .bar::after{ background:#ef4444; }
    .toast.info    .bar::after{ background:#3b82f6; }
    .toast.warning .bar::after{ background:#f59e0b; }

    .toast .bar::after{
      content:""; display:block; height:100%; width:100%;
      transform-origin:left; animation:deplete var(--dur, 5000ms) linear forwards;
    }

    @keyframes slide-in{ to{ transform:translateY(0); opacity:1; } }
    @keyframes deplete{ to{ transform:scaleX(0); } }

    @media (max-width: 480px){
      .toast-wrap{ right:12px; bottom:12px; }
      .toast{ min-width:260px; font-size:13.5px; }
    }
  `]
})
export class ToastContainerComponent {
  constructor(public svc: ToastService) {}
}
