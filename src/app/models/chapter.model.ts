export interface Chapter {
    id: number;
    serialId: number;
    title: string;
    link: string;
    added: Date;
    nextChLinkXPath: string;
    secondaryNextChLinkXPath: string;
    otherNextChLinkXPaths: string;
    isLastChapter: boolean;
    reviewStatus: boolean;
  }
  