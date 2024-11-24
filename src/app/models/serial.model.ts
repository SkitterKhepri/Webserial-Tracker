import { Chapter } from './chapter.model';
import { Author } from './author.model';
import { SerialStatuses } from '../enums/serial-statuses.enum';

export interface Serial {
  id: number;
  title: string;
  status: SerialStatuses;
  authorId: number;
  firstCh: string;
  home: string;
  banner: string;
  nextChLinkXPath?: string;
  secondaryNextChLinkXPath?: string;
  otherNextChLinkXPaths?: string;
  titleXPath?: string;
  reviewStatus: boolean;
  
  chapters: Chapter[];
  author: Author;
}
